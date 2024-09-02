using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{

    public abstract class AbstractHighlighter : MonoBehaviour,
        IIsAwakeProvider,
        IInteractionHighlight,
        ISleep,
        ILocatableBeacon,
        IClickBeacon,
        ITargetRendererProvider
    {
        private bool _enabled = true;

        public TurnOnOffDirectiveType OnSleepDirective;

        public GameObject TargetRenderer;

        Renderer ITargetRendererProvider.GetRenderer() 
        {
            return this.TargetRenderer.GetComponent<Renderer>();
        }

        protected virtual void Awake()
        {
            if (this.TargetRenderer == null)
            {
                this.TargetRenderer = this.gameObject;
            }
        }

        public Vector3 GetApparentPosition() { return this.GetHighlightPosition(); }
        protected abstract Vector3 GetHighlightPosition();


        #region  IInteractionHighlight

        public void Highlight(bool isOn)
        {
            if (!this._enabled)
            {
                return;
            }
            this._SetState(isOn ? EClickBeaconState.IsNextClick : EClickBeaconState.Off);
        }

        #endregion

        public void SetState(EClickBeaconState mode)
        {
            this._SetState(mode);
        }

        void _SetState(EClickBeaconState mode)
        {
            this.SetHighlightState(mode);
        }

        protected abstract void SetHighlightState(EClickBeaconState state);

        public void SetIsAwake(bool isAwake)
        {
            this._enabled = isAwake;

            if (!isAwake)
            {
                if (this.OnSleepDirective != TurnOnOffDirectiveType.DoNothing)
                {
                    this._SetState(this.OnSleepDirective == TurnOnOffDirectiveType.TurnOn ? EClickBeaconState.IsNextClick : EClickBeaconState.Off);
                }
            }
        }

        public bool GetIsAwake()
        {
            return this._enabled;
        }

    }
}

