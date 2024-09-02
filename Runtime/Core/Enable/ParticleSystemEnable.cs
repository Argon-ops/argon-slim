using DuksGames.Argon.Adapters;
using UnityEngine;

namespace DuksGames.Argon.Core
{

    public class ParticleSystemEnable : AbstractThresholdInterpreter
    {
        public ParticleSystem ParticleSystem;

        public bool ToggleGameObject;

        protected override bool GetState()
        {
            return this.ParticleSystem.isPlaying;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            if (isEnabled)
            {
                if (this.ToggleGameObject)
                    this.ParticleSystem.gameObject.SetActive(true);
                this.ParticleSystem.Play();
                return;
            }

            if (this.ToggleGameObject)
                this.ParticleSystem.gameObject.SetActive(false);
            this.ParticleSystem.Stop();
        }
    }
}