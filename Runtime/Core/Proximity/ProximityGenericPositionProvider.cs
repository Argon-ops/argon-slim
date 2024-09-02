using UnityEngine;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class ProximityGenericPositionProvider : MonoBehaviour, ISluggishProximityPosition
    {
        System.Func<Vector3> _getPosition;

        void Awake()
        {
            this.Setup();
        }

        void Setup()
        {
            if (this.GetComponent<Renderer>() != null)
            {
                var renderer = this.GetComponent<Renderer>();
                this._getPosition = () => renderer.bounds.center;
                return;
            }
            this._getPosition = () => this.transform.position;
        }

        public Vector3 GetWorldPosition()
        {
            if (this._getPosition == null)
            {
                this.Setup();
            }
            return this._getPosition();
        }
    }
}