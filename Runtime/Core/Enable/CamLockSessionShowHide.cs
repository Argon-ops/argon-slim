using UnityEngine;
using UnityEditor;
using System;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Core
{
    public class CamLockSessionShowHide : MonoBehaviour
    {
        public Transform ShowRoot;
        public Transform HideRoot;

        void OnEnable()
        {
            this.GetComponent<CamLockSessionEnable>().OnInteractionBegin.AddListener(this.HandleBegin);
            this.GetComponent<CamLockSessionEnable>().OnInteractionEnded.AddListener(this.HandleEnded);

            this.EndSession();
        }

        void OnDisable()
        {
            this.GetComponent<CamLockSessionEnable>().OnInteractionBegin.RemoveListener(this.HandleBegin);
            this.GetComponent<CamLockSessionEnable>().OnInteractionEnded.RemoveListener(this.HandleEnded);
        }

        private void HandleBegin(GameObject arg0)
        {
            if (this.ShowRoot)
                this.ShowRoot?.gameObject.SetActive(true);
            if (this.HideRoot)
                this.HideRoot?.gameObject.SetActive(false);
        }

        void EndSession()
        {
            if (this.ShowRoot)
                this.ShowRoot?.gameObject.SetActive(false);
            if (this.HideRoot)
                this.HideRoot?.gameObject.SetActive(true);
        }

        private void HandleEnded(PlayerInteractionResult arg0)
        {
            this.EndSession();
        }
    }
}