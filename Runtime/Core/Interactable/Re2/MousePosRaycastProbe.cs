using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Core
{
    public class MousePosRaycastProbe : MonoBehaviour
    {
        bool _shouldStop;
        RaycastHit _hit;

        public float CastDistance = 20000;

        public void SetLayerMask(params string[] layerNames)
        {
            this.PickableMask = LayerMask.GetMask(layerNames);
        }

        public int PickableMask { get; private set; } = Physics.DefaultRaycastLayers;

        public void Begin(Action<RaycastHit, bool> hitCallback, Camera cam)
        {
            StartCoroutine(this.Probe(hitCallback, cam));
        }

        public void Stop()
        {
            this._shouldStop = true;
        }

        IEnumerator Probe(Action<RaycastHit, bool> hitCallback, Camera cam)
        {
            while (!this._shouldStop)
            {
                this.Tick(hitCallback, cam);
                yield return null;
            }
        }

        public void Tick(Action<RaycastHit, bool> hitCallback, Camera cam)
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out this._hit, this.CastDistance, this.PickableMask);
            hitCallback(_hit, isHit); 
        }
    }
}