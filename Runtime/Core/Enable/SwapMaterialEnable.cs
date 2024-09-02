using UnityEngine;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public class SwapMaterialEnable : AbstractThresholdInterpreter
    {
        public Material HighlightMat;
        Material GetHighlightMat()
        {
            if (this.HighlightMat == null)
            {
                this.HighlightMat = DuksGameObjectHelper.FindInProject<Material>("Highlighter");
            }
            return this.HighlightMat;
        }
        Material _offMaterial;
        Renderer _targetRenderer => this.GetComponent<Renderer>();

        protected void Awake()
        {
            this._offMaterial = this._targetRenderer.GetComponent<Renderer>().material;
        }

        protected override bool GetState()
        {
            return this._targetRenderer.material == this.GetHighlightMat();
        }

        protected override void SetOnOff(bool isEnabled)
        {
            this._targetRenderer.material = isEnabled ? this.GetHighlightMat() : this._offMaterial;
        }

    }
}