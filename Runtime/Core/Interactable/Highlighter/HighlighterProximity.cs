using UnityEngine;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;
using UnityEngine.Assertions;
using System;

namespace DuksGames.Argon.Core
{
    public class HighlighterProximity : MonoBehaviour,
        ISluggishProximityCallback,
        ISluggishProximityPosition,
        IWillGetDestroyedByDestroyList
    {
        /// <summary>
        /// We are phasing out the concept of proximity; and this class is getting phased out too.
        /// </summary>

        public ChoosableClickBeacon Choosable;
        // IProximityClickManager _manager;

        public Component Highlighter;
        public IIsAwakeProvider IsAwakeProvider => (IIsAwakeProvider)Highlighter;
        ITargetRendererProvider _targetRenderer => (ITargetRendererProvider)Highlighter;
        

        // bool _findable = true;

        public void SetFindable(bool findable)
        {
            return;
            // this._findable = findable;

            // if(!this._findable)
            // {
            //     this._manager.Remove(this.Choosable);
            // }
        }

        void Start()
        {
            // this._manager = null; // SceneServices.Instance.ProximityClickManager;
        }

        public void HandleSluggishProximityUpdate(float distanceSquared, float radiusSquared, GameObject proximityOwner)
        {
            //Debug.LogWarning("Deprecated");
            return;
            // if(!this._findable)
            // {
            //     return;
            // }

            // Assert.IsFalse(this._manager == null, "null ProximityClickManager");
            // // Respect highlighters that don't want to interact right now
            // if (!this.IsAwakeProvider.GetIsAwake())
            // {
            //     this._manager.Remove(this.Choosable);
            //     return;
            // }

            // if (distanceSquared < radiusSquared)
            // {
            //     if (this.IsInCameraFrustum())
            //     {
            //         this._manager.Add(this.Choosable);
            //         return;
            //     }
            // }
            // this._manager.Remove(this.Choosable);
        }

        private bool IsInCameraFrustum()
        {
            if(!Camera.current)
            {
                return false;
            }
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.current);
            return GeometryUtility.TestPlanesAABB(planes, this._targetRenderer.GetRenderer().bounds);
        }

        public Vector3 GetWorldPosition()
        {
            return this.Choosable.GetLocatableBeacon().GetApparentPosition();
        }

        public void WillGetDestroyedByDestroyList()
        {
            return;
            // this._manager.Remove(this.Choosable);
        }
    }
}