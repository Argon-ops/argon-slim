using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Event;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{

    // Must match the enum in the blender script
    public enum CommandBehaviourType
    {
        RestartForwards,
        ToggleAndRestart,
        FlipDirections,
        RestartBackwards,
    }

    public class PlayPlayableCommand : AbstractOvertimeCommand, INormalizedProgressProvider, IDDebugGetClipWrapper
    {
        public PlayableClipWrapper Playable;

        void Start()
        {
            this.SetupWrapper();
            this.StartListening();
        }

        void OnDestroy()
        {
            this.Teardown();
            this.StopListening();
        }

        class PlayCommandCompletion
        {
            public TaskCompletionSource<CommandResult> Completion;
            public CommandInfo Info;
        }

        CursorDataListener OnEndedListener;
        PlayCommandCompletion PlayCompletion = new();

        protected bool IsNotPlaying
        {
            get
            {
                return this.PlayCompletion.Completion == null ||
                    this.PlayCompletion.Completion.Task.Status > TaskStatus.Running;
            }
        }

        protected void InterruptPreviousPlay()
        {
            if (this.IsNotPlaying)
            {
                // Debug.Log($"PPCommand. Wasn't playing ({this.name}) . Comp null? {this.PlayCompletion.Completion == null}");
                return;
            }

            try
            {
                this.PlayCompletion.Completion.TrySetResult(new CommandResult
                {
                    CommandInfo = this.PlayCompletion.Info,
                    Type = CommandResultType.Interrupted
                });

            }
            catch (System.ObjectDisposedException)
            {
                Debug.LogWarning($" object disposed exception from {this.name}");
            }
        }

        protected virtual void HandlePlayEnded(LimitNotifyPlayable.PlayCursorData data)
        {
            // Debug.Log($"HANDLE PLAY ENDED null check: {this.PlayCompletion == null} | playCompl.Completion: {this.PlayCompletion?.Completion == null}".Blue());

            if (this.IsNotPlaying) // this.PlayCompletion.Completion.Task.Status > TaskStatus.Running)
            {
                return;
            }
            this.Playable.Stop();
            try
            {
                this.PlayCompletion.Completion.TrySetResult(new CommandResult
                {
                    CommandInfo = this.PlayCompletion.Info,
                    Type = CommandResultType.Completed
                });
            }
            catch (System.ObjectDisposedException)
            {
                Debug.LogWarning($" ob disposed exception in HandlePlayEnded from {this.name}");
            }
        }

        protected virtual void RestartPlay(CommandInfo commandInfo)
        {
            //Debug.Log($"Restrt Play completion null?: {this.PlayCompletion.Completion == null}".Pink());

            switch (this.BehaviourType)
            {
                case CommandBehaviourType.ToggleAndRestart:
                    var shouldPlayForwards = this.Playable.GetNormalizedProgress01() < .5d;
                    this.Playable.RestartPlay(shouldPlayForwards);
                    break;
                case CommandBehaviourType.FlipDirections:
                    bool shouldFlipToNegative = this.Playable.IsPlaying ?
                            this.Playable.GetSpeed() > 0d :
                            this.Playable.GetNormalizedProgress01() > .5d;
                    Debug.Log($"{this.Playable.Graph.GetEditorName()} Should Flip: {shouldFlipToNegative} | isPlaying: {this.Playable.IsPlaying} | Speed: {this.Playable.GetSpeed()} | Progress: {this.Playable.GetNormalizedProgress01()}".Orange());

                    this.Playable.SetSpeedSign(!shouldFlipToNegative);
                    this.Playable.SetPlaybackTime(this.Playable.GetPlaybackTime()); // seek no where?
                    this.Playable.ClampTimeToBounds();
                    this.Playable.Play();
                    break;
                // case PPCSignalInterpretType.FollowSignalDirectionAndPlay: // TODO: We just need an enum for this
                //     this.playable.SetSpeed(commandInfo.signal > .5f ? 1d : -1d);
                //     this.playable.Play();
                //     break;
                case CommandBehaviourType.RestartForwards: 
                default:
                    this.Playable.RestartPlay(true);
                    break;
                case CommandBehaviourType.RestartBackwards:
                    this.Playable.RestartPlay(false);
                    break;
            }
        }

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            this.InterruptPreviousPlay();
            // Debug.Log($"_EXECUTE: Completion null: {this.PlayCompletion.Completion == null}");
            this.PlayCompletion.Completion = new TaskCompletionSource<CommandResult>();
            // Debug.Log($"Completion null: {this.PlayCompletion.Completion == null}".Pink());

            this.PlayCompletion.Info = commandInfo;

            this.RestartPlay(commandInfo);

            return this.PlayCompletion.Completion.Task;
        }


        void StartListening()
        {
            if (this.OnEndedListener != null && this.Playable.HasOnEndedListener(this.OnEndedListener))
            {
                return;
            }
            this.OnEndedListener = this.Playable.AddOnEndedListener(data => this.HandlePlayEnded(data));
        }

        void StopListening()
        {
            if (this.OnEndedListener != null)
            {
                this.OnEndedListener.StopListening();
            }
        }

        void SetupWrapper()
        {
            if (this.Playable.IsBuilt)
            {
                return;
            }
            this.Playable.WakeUp();
        }

        void Teardown()
        {
            this.Playable.DestroyGraph();
        }

        public double GetNormalizedProgress()
        {
            return this.Playable.GetNormalizedProgress01();
        }

        public override double GetSpeed()
        {
            return this.Playable.GetSpeed();
        }

        public PlayableClipWrapper DGetClipWrapper()
        {
            return this.Playable;
        }


    }

}
