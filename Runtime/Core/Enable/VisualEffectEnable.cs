using System.Collections;
using System.Collections.Generic;
using DuksGames.Argon.Adapters;
using UnityEngine;
using UnityEngine.VFX;

namespace DuksGames.Argon.Core
{

    public class VisualEffectEnable : AbstractThresholdInterpreter
    {

        public VisualEffect EffectLink_;
        VisualEffect _effect => (VisualEffect)this.EffectLink_;

        public bool ToggleGameObject;

        VFXSpawnerState _cachedSpawnerState;
        List<string> _effectNames = new();

        protected override bool GetState()
        {
            this._effect.GetSpawnSystemNames(this._effectNames);
            foreach (var id in this._effectNames)
            {

                _effect.GetSpawnSystemInfo(Shader.PropertyToID(id), this._cachedSpawnerState);
                if (this._cachedSpawnerState.playing)
                {
                    return true;
                }

            }
            return false;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            if (isEnabled)
            {
                if (this.ToggleGameObject)
                    this._effect.gameObject.SetActive(true);
                this._effect.Play();
                return;
            }

            this._effect.Stop();
            if (this.ToggleGameObject)
                this._effect.gameObject.SetActive(false);
        }
    }
}