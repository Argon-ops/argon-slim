using System.Linq;
using DuksGames.Argon.Interaction;
using UnityEngine;


namespace DuksGames.Argon.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class InteractionSelfForwarder : MonoBehaviour
    {

        IClickInteractionHandler[] Listeners;

        void Start()
        {
            this.Listeners = this.GetComponents<IClickInteractionHandler>().ToArray();
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