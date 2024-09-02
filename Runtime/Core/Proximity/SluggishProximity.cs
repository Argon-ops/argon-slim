using System.Collections;
using DuksGames.Argon.Config;
using UnityEngine;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.PropertyDrawers;
using System;

// TODO: make a category for widget classes like this one (and a folder)

namespace DuksGames.Argon.Core
{
    public class SluggishProximity : MonoBehaviour, IWillGetDestroyedByDestroyList
    {
        public float Radius = 3f;
        public float MinIntervalSeconds = .25f;
        public float MaxIntervalSeconds = 2f;
        public float DistanceFactor = .1f;

        public bool BoundsForPosition = true; // TEST: TODO don't foist this


        [TypeEnforce(typeof(ISluggishProximityCallback))]
        public Component CallbackLink;
        ISluggishProximityCallback _callback => (ISluggishProximityCallback)this.CallbackLink;

        [TypeEnforce(typeof(ISluggishProximityPosition))]
        public Component SluggishPositionLink;
        ISluggishProximityPosition _positionProvider => (ISluggishProximityPosition)this.SluggishPositionLink;

        Transform _player;
        bool _isCleanupTime;

        void Start()
        {
            this._player = SceneServices.Instance.PlayerProvider.GetPlayer().transform;

            this._isCleanupTime = false;
            // StartCoroutine(this.Poll());
        }

        IEnumerator Poll()
        {
            throw new System.Exception("This shouldn't happen");
            yield return new WaitForEndOfFrame(); // wait a frame to allow other Components start methods to execute

            while (!this._isCleanupTime)
            {
                if (this._positionProvider == null)
                {
                    Debug.Log($"Null pos provider on {this.name}");
                }

                float ds = Vector3.SqrMagnitude(this._positionProvider.GetWorldPosition() - this._player.position);

                this._callback.HandleSluggishProximityUpdate(
                    ds,
                    this.Radius * this.Radius,
                    this.gameObject);

                yield return new WaitForSeconds(Mathf.Lerp(this.MinIntervalSeconds, this.MaxIntervalSeconds, ds / (this.Radius * this.Radius)));
            }

        }

        void OnDestroy()
        {
            this._isCleanupTime = true;
        }

        public void WillGetDestroyedByDestroyList()
        {
        }
    }
}