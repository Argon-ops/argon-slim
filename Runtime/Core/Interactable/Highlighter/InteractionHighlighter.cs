using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{

    // crude highlighter 
    public class InteractionHighlighter : AbstractHighlighter
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
        Renderer _targetRenderer => this.TargetRenderer.GetComponent<Renderer>();

        protected override void Awake()
        {
            base.Awake();
            this._offMaterial = this._targetRenderer.GetComponent<Renderer>().material;
        }

        protected override Vector3 GetHighlightPosition()
        {
            return this._targetRenderer.bounds.center;
        }

        protected override void SetHighlightState(EClickBeaconState state)
        {
            this._targetRenderer.material = state > EClickBeaconState.Visible ? this.GetHighlightMat() : this._offMaterial;
        }


    }
}