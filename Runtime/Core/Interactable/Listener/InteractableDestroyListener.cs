using System.Collections;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using DuksGames.Argon.Interaction;
using UnityEngine;


namespace DuksGames.Argon.Shared
{
    public class InteractableDestroyListener : MonoBehaviour
    {
        public DestroyList DestroyList;

        public void HandleInteraction(InteractionHandlerInfo clickInteractionHandler)
        {
            this.DestroyList.DestroyMembers();
        }
    }
}