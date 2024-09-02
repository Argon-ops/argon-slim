using UnityEngine;
using DuksGames.Argon.Adapters;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    [RequireComponent(typeof(Collider))]
    public class SliderCollider : MonoBehaviour
    {

        BoxCollider Collider;

        public List<Component> ISignalHandlerLinks = new List<Component>();
        IEnumerable<ISignalHandler> SlidableMechanisms => this.ISignalHandlerLinks.Select(link => (ISignalHandler)link);

        public int axis; // x=0,y=1,z=2
        public bool invert;

        void Start()
        {
            this.Collider = this.GetComponent<BoxCollider>();
        }

        void OnTriggerStay(Collider other)
        {
            // other collider's position relative to our box collider's min in our local space
            var minLocal = this.Collider.transform.InverseTransformPoint(this.Collider.bounds.center)
                                - this.Collider.transform.InverseTransformDirection(this.Collider.size / 2f).Abs();
            var otherLocal = this.Collider.transform.InverseTransformPoint(other.transform.position);
            var difference = otherLocal - minLocal;

            var signal = difference[this.axis] / this.Collider.size[this.axis];
            signal = this.invert ? 1f - signal : signal;

            foreach (var handler in this.SlidableMechanisms)
            {
                handler.HandleISignal(signal);
            }
        }

    }
}