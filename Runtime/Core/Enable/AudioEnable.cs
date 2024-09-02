using UnityEngine;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public class AudioEnable : AbstractThresholdInterpreter
    {
        public AudioClip clip;
        public bool looping;


        [SerializeField]
        PlayableClipWrapper _wrapper;
        CursorDataListener cursorDataListener;
        public bool onSignalAlwaysRestarts;

        #region unity-functions

        void Start()
        {
            this.WakeUpWrapper();
            this.SetupLooping();
        }

        // void OnEnable() {
        //     this.WakeUpWrapper();
        //     this.SetupLooping();
        // }

        // void OnDisable() {
        //     this.StopLooping();
        // }

        void OnDestroy()
        {
            this.StopLooping();
            this.DestroyGraph();
        }

        #endregion

        void WakeUpWrapper()
        {
            if (this._wrapper && this._wrapper.IsBuilt)
            {
                // fend off over calling
                return;
            }
            var audioSource = this.transform.GetOrAddComponent<AudioSource>();

            this._wrapper = this.transform.GetOrAddComponent<PlayableClipWrapper>();
            this._wrapper.Ingredients = new RTPlayableClipIngredients
            {
                AudioClip = this.clip,
                AudioSource = audioSource,
                ShouldLoopAudio = this.looping,
                GraphName = $"AE_{this.clip.name}_{this.transform.name}",
            };
            this._wrapper.WakeUp();
        }

        void SetupLooping()
        {
            // if(!this.looping) {
            //     return;
            // }

            if (this.cursorDataListener != null && this._wrapper.HasOnEndedListener(this.cursorDataListener))
            {
                return;
            }

            this.cursorDataListener = this._wrapper.AddOnEndedListener(data =>
            {

                Debug.Log($"Audio Enable restart OnEnded. norm prog: {this._wrapper.GetNormalizedProgress01()}");
                if (this.looping)
                {
                    this._wrapper.RestartPlay(true);
                    return;
                }
                if (!this.onSignalAlwaysRestarts)
                {
                    this._wrapper.SetPlaybackTime(0d);
                }
            });
        }

        void StopLooping()
        {
            this.cursorDataListener?.StopListening();
        }


        void DestroyGraph()
        {
            this._wrapper.DestroyGraph();
        }


        protected override bool GetState()
        {
            return this._wrapper.IsPlaying;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            if (isEnabled)
            {
                Debug.Log($"CALL RESTART".Green());
                if (!this.onSignalAlwaysRestarts)
                {
                    this._wrapper.Play();
                    return;
                }
                this._wrapper.RestartPlay(true);
                return;
            }
            Debug.Log($"WILL STOP".Pink());
            this._wrapper.Stop();
        }
    }
}