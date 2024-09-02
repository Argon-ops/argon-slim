using UnityEngine;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Gameplay
{
    public class PlayerUpdateLoopRunner : MonoBehaviour, IUpdateLoop
    {
        PlayerCharacterController playerCharacterController;
        void Start()
        {
            this.playerCharacterController = this.GetComponent<PlayerCharacterController>();
        }

        public void DoIUpdateLoop()
        {
            this.playerCharacterController.UpdateLoop();
            CursorLockStateEnforcer.UpdateCursorLock();
        }
    }
}