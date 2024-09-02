using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Core
{
    public class ChoosableClickBeacon : MonoBehaviour, IChoosableClickBeacon
    {

        public Component IClickBeaconLink;
        IClickBeacon _clickBeacon => (IClickBeacon)this.IClickBeaconLink;

        public Component ILocatableBeaconLink;
        ILocatableBeacon _locatableBeacon => (ILocatableBeacon)this.ILocatableBeaconLink;

        public bool _HasForwardFaceVector;

        public Vector3 ForwardFaceVector;

        public GameObject GetInteractionHandlerObject()
        {
            // just assume there's a handler on this same gob. 
            return this.gameObject;
        }

        public void SetState(EClickBeaconState mode)
        {
            if (this.IClickBeaconLink == null) { return; }
            this._clickBeacon.SetState(mode);
        }

        public IClickBeacon GetClickBeacon()
        {
            return this._clickBeacon;
        }

        public ILocatableBeacon GetLocatableBeacon()
        {
            return this._locatableBeacon;
        }

        public bool HasForwardFaceVector(ref Vector3 forwardNormal)
        {
            if (!this._HasForwardFaceVector)
            {
                return false;
            }

            forwardNormal = this.ForwardFaceVector;
            return true;
        }
    }
}