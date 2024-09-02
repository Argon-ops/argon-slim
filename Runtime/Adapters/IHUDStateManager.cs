using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Adapters
{
    public enum HUDStateRequest
    {
        Hide, Show

        // TODO: we may need hud modes actually
        //   in some cases, we want to show the held item
        //     in other cases we don't; for example.
    }

    public struct HUDUpdateInfo
    {
        public HUDStateRequest Request;
    }

    public interface IHUDStateEventDispatcher
    {
        UnityEvent<HUDUpdateInfo> OnHUDRequest { get; }
    }

    public interface IHUDStateManager
    {
        void RequestHUDState(HUDStateRequest hUDStateRequest);
    }

}