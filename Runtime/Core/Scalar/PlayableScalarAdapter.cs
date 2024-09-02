using UnityEngine;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Animate;

namespace DuksGames.Argon.Core
{

    public class PlayableScalarAdapter : MonoBehaviour, ISignalHandler
    {
        public PlayableClipWrapper clipWrapper;

        void Start()
        {
            this.clipWrapper.WakeUp();
        }

        void OnDestroy()
        {
            this.clipWrapper.DestroyGraph();
        }

        public void HandleISignal(double signal)
        {
            this.clipWrapper.PoseAtTime(signal * this.clipWrapper.GetDuration());
        }
    }
}