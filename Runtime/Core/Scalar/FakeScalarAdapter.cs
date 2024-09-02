using UnityEngine;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{

    /// <summary>
    /// Used for testing
    /// </summary>
    public class FakeScalarAdapter : MonoBehaviour, ISignalHandler
    {

        public bool HasEverReceivedASignal { get; private set; } = false;
        public double LastSignal { get; private set; } = 0d;

        [SerializeField] double DisplayLastSignal;


        public void HandleISignal(double signal)
        {
            this.HasEverReceivedASignal = true;
            this.LastSignal = signal;
            this.DisplayLastSignal = this.LastSignal;
        }
    }
}