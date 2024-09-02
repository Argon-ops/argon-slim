using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;
using System;
using System.Collections;

namespace DuksGames.Argon.Core
{
    public class HighlighterDowntime : MonoBehaviour
    {
        public ClickInteractionHandler Handler;
        public AbstractHighlighter Highlighter;

        public float Downtime;

        void OnEnable()
        {
            this.Handler.OnInteracted.AddListener(this.OnInteracted);
        }

        void OnDisable()
        {
            this.Handler.OnInteracted.RemoveListener(this.OnInteracted);
        }

        private void OnInteracted(InteractionHandlerInfo arg0)
        {
            StartCoroutine(this.StartDowntime());
        }

        private IEnumerator StartDowntime()
        {
            // TODO: this should probaby have a check-overlapping-calls mechanism
            this.Highlighter.SetIsAwake(false);
            yield return new WaitForSeconds(this.Downtime);
            this.Highlighter.SetIsAwake(true);
        }
    }
}