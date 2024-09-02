using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Config;
using DuksGames.Argon.Adapters;
using System;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    public class HUDStateListener : MonoBehaviour
    {
        bool _wasActive;

        void Start()
        {
            SceneServices.Instance.HUDStateEventDispatcher.OnHUDRequest.AddListener(OnHUDRequest);
            this._wasActive = this.gameObject.activeSelf;
        }

        void OnDestroy()
        {
            SceneServices.Instance?.HUDStateEventDispatcher?.OnHUDRequest?.RemoveListener(OnHUDRequest); 
        }

        private void OnHUDRequest(HUDUpdateInfo info)
        {
            Debug.Log($"OnHUD Req {info.Request} ".Green());
            if(info.Request == HUDStateRequest.Hide)
            {
                this._wasActive = this.gameObject.activeSelf;
                this.gameObject.SetActive(false);
                return;
            }

            Debug.Log($"Will show: {this._wasActive}");

            this.gameObject.SetActive(this._wasActive);
        }
    }
}

// TODO: not here:
//  A: a (better) hold-item-in-hand set up and a deploy / attack animation
//   B: an animation transition on Siren from attack to recoil, if you attack with the correct item.
//     B2: a slight delay before starting attack anim: maybe player is locked in place at this time?

// C: Actually decide what the backstory is (you're a private eye presumably)