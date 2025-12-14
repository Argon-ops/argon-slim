using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;
using System;
// using DuksGames.LogicBlocks;
using UnityEngine.Events;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;
using System.Text;




namespace DuksGames.Argon.Animate
{

    using CursorEventCallback = System.Action<LimitNotifyPlayable.PlayCursorData>;

    [System.Serializable]
    public class PlayableClipIngredients
    {
        public string GraphName;
        public AnimationClip[] AnimationClips;
        public GameObject[] AnimationTargets; // e.g. armatures
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public bool ShouldLoopAudio;
        public bool AudioAlwaysForwards;


        public static bool IsEquivalent(RTPlayableClipIngredients rtPci, PlayableClipIngredients pci)
        {
            if (!ArrayHelper.BothNullOrSequenceEqual(rtPci.AnimationClips, pci.AnimationClips))
            {
                return false;
            }

            if (!ArrayHelper.BothNullOrSequenceEqual(rtPci.AnimationTargets, pci.AnimationTargets))
            {
                return false;
            }

            if (rtPci.AudioSource != pci.AudioSource) { return false; }

            return true;
        }

        // create a messy name based on ingredients
        public static string GenerateName(PlayableClipIngredients pci)
        {
            var clipNames = pci.AnimationClips.JoinSelf(c =>
            {
                return $"{c.name}";
            });
            clipNames = clipNames.Length > 30 ?
                    $"{clipNames.SubstringRangeSafe(0, 30)}..{clipNames.SubstringFromEndRangeSafe(3)}" :
                    clipNames;
            var audioName = pci.AudioClip == null ? "" : pci.AudioClip.name;
            var targetNames = pci.AnimationTargets.JoinSelf(t => $"{t.name.SubstringRangeSafe(0, 5)}{t.name.SubstringFromEndRangeSafe(3)}");

            return $"PCH__{clipNames}-{audioName}-{targetNames}".SubstringRangeSafe(0, 66);
        }
    }

    [System.Serializable]
    public class RTPlayableClipIngredients
    {
        public string GraphName;
        public AnimationClip[] AnimationClips;

        // We need AnimationTargets only during import -- not runtime. 
        // They allow us to know that an RT_PCI and a PCI are truly equal / equivalent
        //   when we are determining whether to provide existing or make new
        public GameObject[] AnimationTargets;


        public Animator Animator;
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public bool ShouldLoopAudio;
        public bool AudioAlwaysForwards;

        public static RTPlayableClipIngredients From(PlayableClipIngredients p)
        {
            var rt = new RTPlayableClipIngredients
            {
                GraphName = p.GraphName,
                AudioClip = p.AudioClip,
                AudioSource = p.AudioSource,
                ShouldLoopAudio = p.ShouldLoopAudio,
                AudioAlwaysForwards = p.AudioAlwaysForwards,
                AnimationTargets = p.AnimationTargets
            };
            return rt;
        }

    }

    [System.Serializable]
    public class PlayableClipWrapper : MonoBehaviour, IScalarProvider
    {
        public void WakeUp()
        {
            PlayableClipWrapper.SetupGraph(this);
        }

        public bool IsBuilt => this.Graph.IsValid();

        #region events

        void OnAnimationEnded()
        {
            this.OnPlayEnded.Invoke(this);
        }

        public UnityEvent<PlayableClipWrapper> OnPlayEnded;

        #endregion

        public RTPlayableClipIngredients Ingredients;
        public Animator Animator => this.Ingredients.Animator;

        public PlayableGraph Graph { get; private set; }
        CursorDataListener AudioLimitListener;
        PlayableOutput AudioOutput;

        // [SerializeField] 
        bool IsAudioAlwaysForwards => this.Ingredients.AudioAlwaysForwards;

        #region play-speed

        public double PlaybackSpeedAbsolute = 1d;

        #endregion

        #region setup   


        static void AddLimitNotify(PlayableGraph graph, Playable mixerOrClip, PlayableOutput output)
        {
            var limitNotifyPlayable = ScriptPlayable<LimitNotifyPlayable>.Create(graph, 1);
            limitNotifyPlayable.SetDuration(mixerOrClip.GetDuration());

            graph.Connect(mixerOrClip, 0, limitNotifyPlayable, 0);
            limitNotifyPlayable.SetInputWeight(0, 1f);

            output.SetSourcePlayable(limitNotifyPlayable, 0);
        }


        static void SetupGraph(PlayableClipWrapper wrapper)
        {

            // NOTE: This class treats audio outputs as less relevant than animation outputs when 
            //  determining overall duration.
            //   the only exception is when there is no animation clip; only audio.
            //    Animation is the priority when it comes to getting
            //     notified of animation ending and determining normalized progress01.

            var ingredients = wrapper.Ingredients;
            var animator = wrapper.Animator;
            var animationClips = ingredients.AnimationClips;
            var audioSource = ingredients.AudioSource;
            var audioClip = ingredients.AudioClip;
            var graph = PlayableGraph.Create(ingredients.GraphName + "_graph_" + UnityEngine.Random.Range(3, 1000));

            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // Nullable<PlayableOutput> animatorOutput_ = null, audioOutput_ = null;
            // Nullable<Playable> mixerPlayable_ = null, audioPlayable_ = null;

            if (animationClips != null && animationClips.Length > 0)
            {
                var mixer = AnimationMixerPlayable.Create(graph, animationClips.Length);

                var longestClip = animationClips.Aggregate<AnimationClip, AnimationClip>
                        (animationClips[0], (agg, animClp) => animClp.length > agg.length ? animClp : agg);

                for (int i = 0; i < animationClips.Length; ++i)
                {
                    var clip = animationClips[i];

                    if (clip == null)
                    {
                        Debug.LogWarning($"{wrapper.transform.name} Got a null clip at index {i}");
                        continue;
                    }

                    var clipPlayable = AnimationClipPlayable.Create(graph, clip);
                    clipPlayable.SetDuration(clip.length);

                    graph.Connect(clipPlayable, 0, mixer, i);
                    mixer.SetInputWeight(i, 1f);
                }

                mixer.SetDuration(animationClips.
                        Aggregate<AnimationClip, float>(0f, (result, animClip) => Mathf.Max(result, animClip.length)));

                var animatorOutput = AnimationPlayableOutput.Create(graph, "AnimatorOut", animator);

                PlayableClipWrapper.AddLimitNotify(graph, mixer, animatorOutput);
            }

            if (audioClip)
            {
                Assert.IsFalse(audioSource == null, "This PlayabeClipWrapper requires an audio source component.");
                var audioOutput = AudioPlayableOutput.Create(graph, "Audio", audioSource);
                var audioPlayable = AudioClipPlayable.Create(graph, audioClip, true);
                audioPlayable.SetDuration(audioClip.length);
                audioOutput.SetSourcePlayable(audioPlayable);

                bool graphHasNoAnimations = graph.GetOutputCountByType<AnimationPlayableOutput>() == 0;
                if (graphHasNoAnimations || !ingredients.ShouldLoopAudio)
                {
                    // add a limit-aware playable to the audio clip playable
                    PlayableClipWrapper.AddLimitNotify(graph, audioPlayable, audioOutput);
                }

                if (!ingredients.ShouldLoopAudio)
                {
                    // audio just repeats if we don't intervene
                    wrapper.AudioLimitListener = CursorDataListener.Create(
                        audioOutput,
                        (LimitNotifyPlayable.PlayCursorData data) =>
                        {
                            wrapper.TraverseInputsRecursive(audioOutput.GetSourcePlayable(), p =>
                            {
                                p.Pause();
                            });
                        });
                    wrapper.AudioOutput = audioOutput;
                }

            }

            // In some cases, grphs get instantiated and then overwritten.
            //  make sure we destroy any existing graph.
            if (wrapper.Graph.IsValid())
            {
                wrapper.Graph.Destroy();
            }

            wrapper.Graph = graph;
        }


        public void DestroyGraph()
        {
            this._DestroyGraph();
        }

        void _DestroyGraph()
        {

            this.AudioLimitListener?.StopListening();
            if (!this.Graph.IsValid())
            {
                return;
            }
            this.Graph.Destroy();
        }

        public void OnDestroy()
        {
            this._DestroyGraph();
        }


        #endregion

        Playable GetFirstPlayable()
        {
            if (this.Graph.IsValid())
            {
                return this.Graph.GetOutput(0).GetSourcePlayable();
            }
            throw new System.Exception($"Attempt to get a playable from an invalid graph. From game object: {this.name}");
        }

        Nullable<PlayableOutput> LongestDurationOutput()
        {
            Nullable<PlayableOutput> output = null;
            double longest = 0d;
            foreach (var outp in this.Outputs())
            {
                if (outp.GetSourcePlayable().GetDuration() > longest)
                {
                    output = outp;
                    longest = outp.GetSourcePlayable().GetDuration();
                }
            }
            return output;
        }

        Nullable<PlayableOutput> LongestDurationAnimationOutput()
        {
            Nullable<PlayableOutput> output = null;
            double longest = 0d;
            foreach (var outp in this.AnimationOutputs())
            {
                if (outp.GetSourcePlayable().GetDuration() > longest)
                {
                    output = outp;
                    longest = outp.GetSourcePlayable().GetDuration();
                }
            }
            return output;
        }

        string DOutputInfo()
        {
            StringBuilder builder = new();
            builder.Append("ANIM outputs: \n");
            foreach(var outp in this.AnimationOutputs())
            {
                builder.Append($"{outp.GetSourcePlayable().IsNull()}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// This is a band-aid, duct tape solution. Taping over the fact that we
        ///   consider the animation to be the 'main' output and its usually the one we want
        ///     but not if there are no animations--just audio. 
        ///       All this is to make GetNormalizedProgress01 make sense in all cases
        ///       TODO: strongly consider not supporting GetNormalizedProgress01. Instead
        ///         separate progress for animation and audio and let outside classes decide what they mean by progress.
        /// </summary>
        /// <returns></returns>
        Nullable<PlayableOutput> LongestRelevantOutputFavorAnimation()
        {
            var longestAnimOutput = this.LongestDurationAnimationOutput();
            if (longestAnimOutput != null)
                return longestAnimOutput;

            Nullable<PlayableOutput> output = null;
            double longest = 0d;
            foreach (var outp in this.AudioOutputs())
            {
                if (outp.GetSourcePlayable().GetDuration() > longest)
                {
                    output = outp;
                    longest = outp.GetSourcePlayable().GetDuration();
                }
            }
            return output;
        }

        AnimationClip LongestDurationClip()
        {
            var animationPlayable = this.LongestDurationOutput();
            if (animationPlayable == null)
            {
                return null;
            }
            var apl = (AnimationClipPlayable)animationPlayable.Value.GetSourcePlayable();
            return apl.GetAnimationClip();
        }

        public double GetDuration()
        {
            var longest = this.LongestRelevantOutputFavorAnimation(); // this.LongestDurationOutput();
            if (longest == null) { return 0d; }
            return longest.Value.GetSourcePlayable().GetDuration();
        }

        public double GetNormalizedProgress01()
        {
            var longest = this.LongestRelevantOutputFavorAnimation(); // this.LongestDurationOutput();
            if (longest == null) { return 0d; }
            var ap = (Playable)longest.Value.GetSourcePlayable();
            return ap.GetTime() / ap.GetDuration();
        }

        public IEnumerable<PlayableOutput> Outputs()
        {
            for (int i = 0; i < this.Graph.GetOutputCount(); ++i)
            {
                yield return this.Graph.GetOutput(i);
            }
        }

        public IEnumerable<AnimationPlayableOutput> AnimationOutputs()
        {
            for (int i = 0; i < this.Graph.GetOutputCountByType<AnimationPlayableOutput>(); ++i)
            {
                yield return (AnimationPlayableOutput)this.Graph.GetOutputByType<AnimationPlayableOutput>(i);
            }
        }

        public IEnumerable<AudioPlayableOutput> AudioOutputs()
        {
            for (int i = 0; i < this.Graph.GetOutputCountByType<AudioPlayableOutput>(); ++i)
            {
                yield return (AudioPlayableOutput)this.Graph.GetOutputByType<AudioPlayableOutput>(i);
            }
        }

        public bool IsPlaying => this.Graph.IsPlaying();


        PlayableOutput MainOutput()
        {

            // TODO: consider: should outsiders be allowed to choose which output they want to 
            //   listen for the end of. (Animation or Audio)
            //    TODO: related. there's a bug when the audio is the longest output. If its over twice 
            //       the duration of animation it messes up our calculations regarding flipping directions in PlayPlayableCommand
            //         Because we assume main output and longest are the same thing.
            // return this.Graph.GetOutput(0);
            var result = this.LongestRelevantOutputFavorAnimation();
            Assert.IsTrue(result.HasValue,  $"No value for the nullable of MainOutput. Gameobject: {this.name} with root: {this.transform.root.name}." +
                                            $" Try checking for PlayableClipWrappers whose clips target no-longer-existent armatures"); 
                                            // TODO: re-create this by making an animation cmd (in Blender) deleting the armature that was animated and they exporting
                                            //  with the cmd still in place. Then look at the resulting graph so that we can see what the symptoms of this are.
                                            //  Goal of catching this earlier.
            
            return (PlayableOutput)result;
           
        }

        public bool HasOnEndedListener(CursorDataListener notificationReceiver)
        {
            foreach (var nr in this.MainOutput().GetNotificationReceivers())
            {
                if (nr == notificationReceiver) { return true; }
            }
            return false;
        }

        public CursorDataListener AddOnEndedListener(CursorEventCallback onPlayEnded)
        {
            
            try
            {
                return CursorDataListener.Create(this.MainOutput(), onPlayEnded);
            }
            catch (System.NullReferenceException nre)
            {
                throw new System.NullReferenceException($"For {this.name} :: {nre.ToString()}");
            }
        }

        public CursorDataOnceListener AddOnEndedOnceListener(CursorEventCallback onPlayEnded)
        {
            return CursorDataOnceListener.Create(this.MainOutput(), onPlayEnded);
        }

        void PrepareAudioForPlay()
        {

            if (!this.AudioOutput.IsOutputValid())
            {
                return;
            }

            this.TraverseInputsRecursive(this.AudioOutput.GetSourcePlayable(), p =>
            {
                if (this.IsAudioAlwaysForwards)
                    p.SetSpeed(this.PlaybackSpeedAbsolute);

                p.Play();
            });

        }

        public void Play()
        {
            this.PrepareAudioForPlay();
            this.Graph.Play();
        }

        public void RestartPlay(bool forwards = true)
        {
            this.SetSpeedSign(forwards);
            this.SetPlaybackTime(forwards ? 0d : this.GetDuration());
            this.Play();
        }

        public void Stop()
        {
            this.Graph.Stop();
        }

        public void SetPlayablesPaused(bool shouldPause)
        {
            foreach (var output in this.Outputs())
            {
                this.TraverseInputsRecursive(output.GetSourcePlayable(), p =>
                {
                    if (shouldPause) p.Pause();
                    else p.Play();
                });
            }
        }

        IEnumerable<Playable> TraverseInputsRecursive(Playable pl)
        {
            Queue<Playable> playables = new Queue<Playable>();
            playables.Enqueue(pl);
            while (playables.Count > 0)
            {
                var p = playables.Dequeue();
                yield return p;
                for (int i = 0; i < p.GetInputCount(); ++i)
                {
                    playables.Enqueue(p.GetInput(i));
                }
            }
        }

        void TraverseInputsRecursive(Playable p, Action<Playable> callback)
        {
            if (!p.IsValid())
            {
                return;
            }
            callback(p);
            for (int i = 0; i < p.GetInputCount(); ++i)
            {
                this.TraverseInputsRecursive(p.GetInput(i), callback);
            }
        }

        Playable GetFurthestInput(Playable p)
        {
            if (p.GetInputCount() == 0) { return p; }
            return this.GetFurthestInput(p.GetInput(0));
        }

        /// <summary>
        /// </summary>
        /// <returns>The current playback position in time</returns>
        public double GetPlaybackTime()
        {
            return this.LongestDurationOutput().Value.GetSourcePlayable().GetTime();
        }

        public void SetPlaybackTime(double t)
        {

            foreach (var animOutput in this.AnimationOutputs())
            {
                var rootPlayable = animOutput.GetSourcePlayable();
                this.TraverseInputsRecursive(rootPlayable, p => p.SetTime(t));
            }

            foreach (var audOutput in this.AudioOutputs())
            {
                double audioPlaybackTime = t;

                if (this.IsAudioAlwaysForwards && this.GetSpeed() < 0d)
                {
                    // this amounts to a guess as to what behaviour user wants: audio position = anim position reflected over .5 and scaled to audio duration
                    double flippedNormalizedTime = 1d - Math.Clamp(t, 0d, this.GetDuration()) / this.GetDuration();
                    audioPlaybackTime = flippedNormalizedTime * audOutput.GetSourcePlayable().GetDuration();
                }

                this.TraverseInputsRecursive(audOutput.GetSourcePlayable(), p => p.SetTime(audioPlaybackTime));

                // TODO: allow the clip to play at the same point in the anim, even when the anim is playing bkwrds
            }
        }

        public void PoseAtTime(double t)
        {
            this.Play();
            this.SetPlayablesPaused(true);
            this.SetPlaybackTime(t);
        }

        public double GetSpeed()
        {
            return this.GetFirstPlayable().GetSpeed();
        }

        public void SetSpeedSign(bool positiveSpeed)
        {
            this.SetSpeed(this.PlaybackSpeedAbsolute * (positiveSpeed ? 1d : -1d)); 
        }

        public void SetSpeed(double s)
        {

            foreach (var animOut in this.AnimationOutputs())
            {
                animOut.GetSourcePlayable().SetSpeed(s);
            }

            double audioSpeed = this.IsAudioAlwaysForwards ? Math.Abs(s) : s;
            foreach (var audOut in this.AudioOutputs())
            {
                audOut.GetSourcePlayable().SetSpeed(audioSpeed); // B
            }
        }

        public void ResetPlayback()
        {
            this.SetPlaybackTime(0f);
        }

        public void MoveToEndOfClip()
        {
            this.SetPlaybackTime(this.GetDuration());
        }

        public void ClampTimeToBounds()
        {
            double time = this.GetFirstPlayable().GetTime();
            if (time < 0d)
            {
                this.SetPlaybackTime(0d);
            }
            if (time > this.GetDuration())
            {
                this.SetPlaybackTime(this.GetDuration());
            }
        }

        public double GetIScalar()
        {
            return this.GetNormalizedProgress01();
        }
    }


}

/*

        // Version before we decided to add a LimitNotify to the longest
        //   output Animation OR Audio
        static void SetupGraph(PlayableClipWrapper wrapper)
        {

            var ingredients = wrapper.Ingredients;
            var animator = wrapper.Animator;
            var animationClips = ingredients.AnimationClips;
            var audioSource = ingredients.AudioSource;
            var audioClip = ingredients.AudioClip;
            var graph = PlayableGraph.Create(ingredients.GraphName + "_graph_" + UnityEngine.Random.Range(3, 1000));

            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            if (animationClips != null && animationClips.Length > 0)
            {
                var mixer = AnimationMixerPlayable.Create(graph, animationClips.Length);

                var longestClip = animationClips.Aggregate<AnimationClip, AnimationClip>
                        (animationClips[0], (agg, animClp) => animClp.length > agg.length ? animClp : agg);

                for (int i = 0; i < animationClips.Length; ++i)
                {
                    var clip = animationClips[i];

                    if (clip == null)
                    {
                        Debug.LogWarning($"{wrapper.transform.name} Got a null clip at index {i}");
                        continue;
                    }

                    var clipPlayable = AnimationClipPlayable.Create(graph, clip);
                    clipPlayable.SetDuration(clip.length);

                    graph.Connect(clipPlayable, 0, mixer, i);
                    mixer.SetInputWeight(i, 1f);
                }

                mixer.SetDuration(animationClips.
                        Aggregate<AnimationClip, float>(0f, (result, animClip) => Mathf.Max(result, animClip.length)));

                var animatorOutput = AnimationPlayableOutput.Create(graph, "AnimatorOut", animator);
                PlayableClipWrapper.AddLimitNotify(graph, mixer, animatorOutput);
            }

            if (audioClip)
            {
                Assert.IsFalse(audioSource == null, "This PlayabeClipWrapper requires an audio source component.");
                var audioOutput = AudioPlayableOutput.Create(graph, "Audio", audioSource);
                var audioPlayable = AudioClipPlayable.Create(graph, audioClip, true);
                audioPlayable.SetDuration(audioClip.length);
                audioOutput.SetSourcePlayable(audioPlayable);

                bool graphHasNoAnimations = graph.GetOutputCountByType<AnimationPlayableOutput>() == 0;
                if (graphHasNoAnimations || !ingredients.ShouldLoopAudio)
                {
                    // add a limit-aware playable to the audio clip playable
                    PlayableClipWrapper.AddLimitNotify(graph, audioPlayable, audioOutput);
                }

                if (!ingredients.ShouldLoopAudio)
                {
                    // audio just repeats if we don't intervene
                    wrapper.AudioLimitListener = CursorDataListener.Create(
                        audioOutput,
                        (LimitNotifyPlayable.PlayCursorData data) =>
                        {
                            wrapper.TraverseInputsRecursive(audioOutput.GetSourcePlayable(), p =>
                            {
                                p.Pause();
                            });
                        });
                    wrapper.AudioOutput = audioOutput;
                }
            }

            // In some cases, grphs get instantiated and then overwritten.
            //  make sure we destroy any existing graph.
            if (wrapper.Graph.IsValid())
            {
                wrapper.Graph.Destroy();
            }

            wrapper.Graph = graph;
        }

*/