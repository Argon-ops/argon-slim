using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class BeaconBuilder : MonoBehaviour
    {
        public enum BeaconPlacementOption
        {
            ObjectPosition,
            ColliderBoundsCenter
        }
        public BeaconPlacementOption beaconPlacementOption;

        public Vector3 normalizedBeaconNudge;
        public bool beaconShouldRotateNinety;

        public GameObject ClickBeaconPrefab;

        public Transform PositionReference;

        void SetPosition(Transform _beacon, BeaconHighlighter beaconHighlighter)
        {
            if(this.PositionReference != null)
            {
                _beacon.position = this.PositionReference.position;
                DestroyImmediate(this.PositionReference.gameObject);
                return;
            }

            switch (this.beaconPlacementOption)
            {
                case BeaconPlacementOption.ObjectPosition:
                default:
                    _beacon.position = beaconHighlighter.TargetRenderer.transform.position;
                    break;
                case BeaconPlacementOption.ColliderBoundsCenter:
                    _beacon.position = this.GetComponent<Collider>().bounds.center;
                    break;
            }

            var scaler = new System.Func<Vector3>(() =>
            {
                if (this.GetComponent<Collider>())
                {
                    var collider = this.GetComponent<Collider>();
                    var enabled = collider.enabled;
                    collider.enabled = true; // disabled colliders will report zero bounds
                    var extents = collider.bounds.extents;
                    collider.enabled = enabled;
                    return extents;
                }
                if (this.GetComponent<Renderer>())
                {
                    return this.GetComponent<Renderer>().bounds.extents;
                }
                return Vector3.one;
            });
            _beacon.position += Vector3.Scale(scaler(), this.normalizedBeaconNudge); // Don't nudge relative unfortunately, it's too confusing for me.

        }

        public Transform CreateBeacon(BeaconHighlighter beaconHighlighter)
        {
            var _beacon = GameObject.Instantiate(this.ClickBeaconPrefab).transform;
            _beacon.name = $"{_beacon.name}_{beaconHighlighter.name}";

            this.SetPosition(_beacon, beaconHighlighter);            

            _beacon.RotateAround(_beacon.transform.position, Vector3.up, this.beaconShouldRotateNinety ? 90f : 0f); // Don't relative rotate, it's too confusing.

            GameObject.Destroy(this); // our job is done! ciao!
            return _beacon;
        }

    }
}
