using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Adapters
{
    public enum ThirdPersonControllerState
    {
        InMenu, InGame, InCamLock
    }

    public struct ThirdPersonControllerStateEvent
    {
        public ThirdPersonControllerState fromState;
        public ThirdPersonControllerState toState;
    }

    public interface IThirdPersonControllerStateManager
    {
        void SetState(ThirdPersonControllerState thirdPersonControllerState);

        UnityEvent<ThirdPersonControllerStateEvent> GetOnChanged();
    }

}