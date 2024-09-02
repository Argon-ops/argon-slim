using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;
using DuksGames.Argon.Utils;
using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class BeaconHighlighter : AbstractHighlighter, IWillGetDestroyedByDestroyList
    {
        Transform _beacon;
        Renderer _beaconRenderer;
        IBeaconDevice _beaconDevice;

        IBeaconDevice GetBeacon()
        {
            if (this._beaconDevice == null)
            { 
                this._beacon = this.GetComponent<BeaconBuilder>().CreateBeacon(this);
                this._beaconDevice = this._beacon.GetComponent<IBeaconDevice>();
            }
            return this._beaconDevice;
        }

        // TODO: not here: re resolve proximity click system and raycast probe...
        //  Or just continue to use raycast probe and really big colliders?

        Renderer GetRenderer()
        {
            if (this._beacon == null)
            {
                this.GetBeacon();
            }
            if(this._beaconRenderer == null)
            {
                this._beaconRenderer = this._beacon.GetComponentInChildren<Renderer>();
            }
            return this._beaconRenderer;
        }

        protected override Vector3 GetHighlightPosition()
        {
            return this.GetRenderer().bounds.center;
        }

        protected override void SetHighlightState(EClickBeaconState state)
        {
            this.GetBeacon().SetDeviceState(state);
        }

        public void WillGetDestroyedByDestroyList()
        {
            GameObject.Destroy(this._beacon.gameObject);
        }
    }
}