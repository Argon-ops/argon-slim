using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;
using System.Linq;
using System.Collections.Generic;
using DuksGames.Argon.PropertyDrawers;

namespace DuksGames.Argon.Core
{
    public class ProximityOmniAdapter : MonoBehaviour, ISluggishProximityCallback
    {
        public Component[] IOnOffLinks;
        public Component[] ISignalHandlerLinks;

        IEnumerable<IOnOffHandler> _onOffHandlers;
        IEnumerable<ISignalHandler> _signalHandlers;

        void Awake()
        {
            this._onOffHandlers = this.IOnOffLinks.Where(c => c is IOnOffHandler).Select(c => (IOnOffHandler)c);
            this._signalHandlers = this.ISignalHandlerLinks.Where(c => c is ISignalHandler).Select(c => (ISignalHandler)c);
        }

        public void HandleSluggishProximityUpdate(float distanceSquared, float radiusSquared, GameObject proximityOwner)
        {
            foreach (var onOff in this._onOffHandlers)
            {
                onOff.HandleIOnOff(distanceSquared < radiusSquared);
            }
            foreach (var signalHandler in this._signalHandlers)
            {
                signalHandler.HandleISignal(distanceSquared / radiusSquared);
            }
        }
    }
}