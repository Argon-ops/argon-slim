using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    /// Detects interactions between the attached collider
    ///    and a trigger collider. 
    ///    If the other collider has an ITriggerEnterHandler, calls HandleTriggerEnter.
    ///    If theo ther collider has an ITriggerExitHandler, calls HandleTriggerExit.
    /// </summary>
    public class TriggerInteractionProbe : MonoBehaviour
    {
        InteractionHandlerInfo handlerInfo;

        void Start()
        {
            handlerInfo = new InteractionHandlerInfo
            {
                Source = Camera.main.gameObject
            };
        }

        void OnTriggerEnter(Collider other)
        {
            var interactionHandler = other.GetComponent<ITriggerEnterHandler>();
            if (interactionHandler == null) { return; }
            // Debug.Log($"Trigger enter with collider: {other.name}");
            interactionHandler.HandleTriggerEnter(handlerInfo);
        }

        void OnTriggerExit(Collider other)
        {
            var interactionHandler = other.GetComponent<ITriggerExitHandler>();
            if (interactionHandler == null) { return; }
            interactionHandler.HandleTriggerExit(handlerInfo);
        }
    }
}