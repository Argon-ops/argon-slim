using UnityEngine;
using DuksGames.Argon.Config;
using DuksGames.Argon.Interaction;
using System.Collections;
using DuksGames.Argon.Adapters;
using UnityEditor.SearchService;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    /// Glue between player clicks and the current click beacon 
    /// </summary>
    public class ProximityClickProbe : MonoBehaviour
    {
        ICurrentBeaconProvider _cachedCurrentBeaconProvider;

        public Camera Camera;
        private EnforceOneClickHandler _enforceOneClickHandler;

        void Start()
        {
            this._cachedCurrentBeaconProvider = SceneServices.Instance.CurrentBeaconProvider;
            this._enforceOneClickHandler = this.GetComponent<EnforceOneClickHandler>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var nextClickable = this._cachedCurrentBeaconProvider.GetCurrentBeacon();

                if (nextClickable == null)
                {
                    return;
                }


                var clickFeedback = nextClickable.GetInteractionHandlerObject().GetComponent<IClickFeedback>();
                clickFeedback?.GiveFeedback();
                
                var handler = nextClickable.GetInteractionHandlerObject().GetComponent<IClickInteractionHandler>();
                // if(!this._enforceOneClickHandler.CanHandle(handler)) 
                // {
                //     return;
                // }

                handler?.Interact(new InteractionHandlerInfo 
                {
                    Source = SceneServices.Instance.PlayerProvider.GetPlayer(),
                    InteractionTarget = (Component)handler
                });
            }
        }
    }

}