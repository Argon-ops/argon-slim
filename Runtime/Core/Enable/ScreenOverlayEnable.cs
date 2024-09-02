using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Interaction;
using UnityEngine.UIElements;

namespace DuksGames.Argon.Core
{
    public class ScreenOverlayEnable : AbstractThresholdInterpreter
    {
        IOverlayEnable _oF;
        IOverlayEnable overlayFeedback
        {
            get
            {
                if (this._oF == null)
                {
                    System.Func<Component> findUIRoot = () =>
                    {
                        var uidoc = GameObject.FindObjectOfType<UIDocument>();
                        if (uidoc) { return uidoc; }
                        var canvas = GameObject.FindObjectOfType<Canvas>();
                        if (canvas) return canvas;
                        throw new System.Exception($"ScreenOverlayEnable on {this.name} didn't find a ui root");
                    };

                    this._oF = findUIRoot().GetComponent<IOverlayEnable>();
                }
                return this._oF;
            }
        }

        public string overlayName;

        protected override void SetOnOff(bool isEnabled)
        {
            this.overlayFeedback.ShowHide(isEnabled, this.overlayName);
        }

        protected override bool GetState()
        {
            return this.overlayFeedback.IsOverlayEnabled(this.overlayName);
        }
    }
}