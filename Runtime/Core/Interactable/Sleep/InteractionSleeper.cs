using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Core
{
    public class InteractionSleeper : MonoBehaviour
    {

        public ClickInteractionHandler target;
        void Start()
        {
            this.target.OnInteracted.AddListener(this.OnInteraction);
        }

        public void OnInteraction(InteractionHandlerInfo clickHander)
        {
            ((ClickInteractionHandler)clickHander.InteractionTarget).SetIsAwake(false);
            // clickHander.SetIsAwake(false);
        }
    }

}