using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;
using UnityEditor.SearchService;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Gameplay
{

   
    public class RaycastClickFinder 
    {
        [HideInInspector] public Camera Cam;

        // reminder: DefaultRaycastLayers are all layers except the IgnoreRaycast layer; (so the '| Picking' is not actually needed).
        int _pickableMask = (Physics.DefaultRaycastLayers | LayerMask.GetMask("Picking")) 
                                    & ~LayerMask.GetMask("CamLockPickable")
                                    & ~LayerMask.GetMask("PhysicsButNotPickable")
                                    & ~LayerMask.GetMask("PlayerMainCollider");

        RaycastHit _hit;

        public float CastDistance = 10f;

        public Transform Current { get; private set; }

        public Transform UpdateCurrent()
        {
            IClickInteractionHandler currentHandler = null;
            InteractionHandlerInfo info = null;

            if (Physics.Raycast(new Ray(this.Cam.transform.position, this.Cam.transform.forward), out this._hit, this.CastDistance, this._pickableMask))
            {
                currentHandler = this._hit.transform.gameObject.GetComponent<IClickInteractionHandler>();
                info = new InteractionHandlerInfo
                {
                    Source = SceneServices.Instance.PlayerProvider.GetPlayer(),
                    InteractionTarget = (Component)currentHandler
                };

                if (currentHandler != null && currentHandler.CouldInteract(info))
                {
                    this.Current = this._hit.transform;
                }
                else 
                {
                    this.Current = null;
                }
            }

            if (currentHandler == null)
            {
                this.Current = null;
            }

            return this.Current;
        }

    }
}
