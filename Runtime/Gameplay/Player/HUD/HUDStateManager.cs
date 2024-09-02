using DuksGames.Argon.Adapters;
using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Gameplay
{
    public class HUDStateManager : MonoBehaviour, IHUDStateManager, IHUDStateEventDispatcher
    {

        [SerializeField] UnityEvent<HUDUpdateInfo> _onHUDRequest;
        public UnityEvent<HUDUpdateInfo> OnHUDRequest => this._onHUDRequest;

        public void RequestHUDState(HUDStateRequest hUDStateRequest)
        {
            this._onHUDRequest.Invoke(new HUDUpdateInfo
            {
                Request = hUDStateRequest,
            });
        }
    }
}