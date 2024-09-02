using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;
using UnityEditor.SearchService;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Gameplay
{

    /// <summary>
    /// Runs simple highlighting: interactables will highlight when 
    ///   the camera's ray intersects them. 
    /// Attach this component to 
    ///   your player's game object if you want simple highlighting. For
    ///   proximity highlighting, use ProximityClickProbe instead.
    /// </summary>
    public class RaycastProbe : MonoBehaviour , ISuspendable
    {
        public Camera Cam;


        int _pickableMask;

        RaycastHit _hit;

        public float CastDistance = 100f;

        [SerializeField] bool _suspended;


        void Start()
        {
            // reminder: DefaultRaycastLayers are all layers except the IgnoreRaycast layer; (so the '| Picking' is not actually needed).
            this._pickableMask = (Physics.DefaultRaycastLayers | LayerMask.GetMask("Picking")) 
                                    & ~LayerMask.GetMask("CamLockPickable")
                                    & ~LayerMask.GetMask("PhysicsButNotPickable")
                                    & ~LayerMask.GetMask("PlayerMainCollider");
        }

        void Update()
        {
            if(this._suspended)
            {
                return;
            }

            IClickInteractionHandler currentHandler = null;
            InteractionHandlerInfo info = null;

            if (!Physics.Raycast(new Ray(this.Cam.transform.position, this.Cam.transform.forward), out this._hit, this.CastDistance, this._pickableMask))
            {
                SceneServices.Instance.OneAtATimeHighlightManager.NextHighlight(null);
            }
            else
            {
                currentHandler = this._hit.transform.gameObject.GetComponent<IClickInteractionHandler>();
                info = new InteractionHandlerInfo
                {
                    Source = SceneServices.Instance.PlayerProvider.GetPlayer(),
                    InteractionTarget = (Component)currentHandler
                };

                if (currentHandler != null && currentHandler.CouldInteract(info))
                {
                    SceneServices.Instance.OneAtATimeHighlightManager.NextHighlight(this._hit.transform.GetComponent<IInteractionHighlight>());
                }
                else 
                {
                    SceneServices.Instance.OneAtATimeHighlightManager.NextHighlight(null);
                }
            }

            // var current = SceneServices.Instance.OneAtATimeHighlightManager.GetCurrent();
            // Debug.Log($"CUR: '{current?.name}'");
            if (currentHandler == null)
            {
                return;
            }


            if (Input.GetMouseButtonDown(0))
            {
                // var handler = currentHandler.GetComponent<IClickInteractionHandler>(); // _hit.transform.GetComponent<IClickInteractionHandler>();
                Debug.Log($"handler {this._hit.transform.gameObject.name}".Pink());
                // if(!this.enforceOneClickHandler.CanHandle(handler)) 
                // {
                //     Debug.Log("deferring to another handler ");
                //     return;
                // }

                currentHandler.Interact(info);
            }

        }

        void ISuspendable.Suspend()
        {
            this._suspended = true;
        }

        void ISuspendable.Restart()
        {
            this._suspended = false;
        }
    }
}


// TODO not here:  Option to designate an object as the FPS player (with Raycast Probe, Trigger Probe, Char Controller, Char Input and whatever else we're forgetting...)
//    Accomplish this with a 'replace with prefab'

// TODO: re2 style item picking
//  RaycastProbe doesn't participate
//    RETwoPickProbe:
//      Curates a collection of nearby, eligible interactables
//      Any that meet the show-beacon criteria: the player has come within x distance of them, 
//         They aren't disabled--should be in show-beacon mode
//          Zero or one of them should be in is-next-click mode, if: player is close enough, player is looking at it 
//            more than at any other
//    Try this after we have a million other things squared away. Seems like a source of fineckiness.
//     
