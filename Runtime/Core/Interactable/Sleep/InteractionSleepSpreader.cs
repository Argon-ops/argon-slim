using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Event;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace DuksGames.Argon.Core
{

    public class InteractionSleepSpreader : MonoBehaviour, ISimpleDestroyReceiver
    {

        public AbstractHighlighter abstractHighlighter;
        public bool ColliderAlso;

        public void HandleIsAwake(bool isAwake)
        {
            if (this.abstractHighlighter != null)
            {
                this.abstractHighlighter.SetIsAwake(isAwake);
            }
            
            if (this.ColliderAlso)
            {
                // TODO: at least set the collider at import time 
                //   and possibly have an option to let the collider live on a 
                //     different object. 
                this.GetComponent<Collider>().enabled = isAwake;
            }
        }
    }
}