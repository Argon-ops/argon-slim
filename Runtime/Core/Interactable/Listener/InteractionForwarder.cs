using System.Linq;
using DuksGames.Argon.Interaction;
using UnityEngine;


namespace DuksGames.Argon.Core
{
    /// <summary>
    /// Glue between a SceneObjectReferencer and an interaction handler (e.g. ClickInteractionHandler)
    /// </summary>
    public class InteractionForwarder : MonoBehaviour
    {
        public SceneObjectsReferencer Referencer;

        IClickInteractionHandler[] Listeners;

        void Start()
        {
            this.Listeners = this.Referencer.Objects.Select(o => o.GetComponent<IClickInteractionHandler>()).ToArray();
        }

        public void HandleInteraction(InteractionHandlerInfo interactionHandlerInfo)
        {
            Debug.Log($"Will forward to {this.Listeners.Length} listeners");
            foreach(var listener in Listeners)
            {
                listener.Interact(interactionHandlerInfo);
            }
        }
    }
}