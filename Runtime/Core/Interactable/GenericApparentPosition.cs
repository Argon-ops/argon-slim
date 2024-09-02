using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class GenericApparentPosition : MonoBehaviour, ILocatableBeacon
    {
        Collider _collider;
        Renderer _renderer;
        void Start()
        {
            this._renderer = this.GetComponent<Renderer>();
            this._collider = this.GetComponent<Collider>();
        }
        public Vector3 GetApparentPosition()
        {
            return this._renderer != null ? this._renderer.bounds.center : this.transform.position;
        }

        public bool IsPointingToBeacon(RaycastHit hit)
        {
            if (this._collider == null)
            {
                return hit.collider.transform == this.transform;
            }
            return hit.collider == this._collider;
        }
    }
}