using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;
using System.Linq;
using UnityEngine.Assertions;
using DuksGames.Argon.Config;
using System;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    public class ThirdPersonControllerStateSuspendListener : MonoBehaviour
    {
        [SerializeField] GameObject[] _InGameSuspendableLinks;
        ISuspendable[] _inGameSuspendables;
        public ThirdPersonControllerStateManager DthirdPersonStateManager;

        void Start()
        {
            this.ListenThirdPersonStates();
            this._inGameSuspendables = this._InGameSuspendableLinks.SelectMany(g => g.GetComponents<ISuspendable>()).ToArray();
            this.SanityCheckSuspendables();
        }


        void ListenThirdPersonStates()
        {
            SceneServices.Instance.ThirdPersonControllerStateManager.GetOnChanged().AddListener(this.OnThirdChanged);
        }

        void SanityCheckSuspendables()
        {
            foreach (var g in this._InGameSuspendableLinks)
            {
                var suspenables = g.GetComponents<ISuspendable>();
                Assert.IsTrue(suspenables.Length > 0, $"No ISuspendables found on {g.name} ");
            }
        }


        public void OnThirdChanged(ThirdPersonControllerStateEvent thirdPersonControllerStateEvent)
        {
            if (thirdPersonControllerStateEvent.toState == ThirdPersonControllerState.InGame)
            {
                foreach (var s in this._inGameSuspendables)
                {
                    s.Restart();
                }
                return;
            }

            foreach (var s in this._inGameSuspendables)
            {
                s.Suspend();
            }
        }
    }
}