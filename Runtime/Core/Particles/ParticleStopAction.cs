using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Core
{
    public class ParticleStopAction : MonoBehaviour
    {
        public UnityEvent<GameObject> OnParticlesStopped;

        void Start()
        {
            var main = this.GetComponent<ParticleSystem>().main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        void OnParticleSystemStopped()
        {
            this.OnParticlesStopped.Invoke(this.gameObject);
        }
    }
}