using UnityEngine;
using DuksGames.Argon.Adapters;
using UnityEngine.Events;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    /// Owns the current ThirdPersonController state: is the player playing 
    ///  or are they paused or are they in a cam lock session, etc.
    /// </summary>
    public class ThirdPersonControllerStateManager : MonoBehaviour, IThirdPersonControllerStateManager
    {
        [SerializeField] UnityEvent<ThirdPersonControllerStateEvent> _onChanged;
        public UnityEvent<ThirdPersonControllerStateEvent> GetOnChanged()
        {
            return this._onChanged;
        }

        ThirdPersonControllerState _current;

        public void SetState(ThirdPersonControllerState thirdPersonControllerState)
        {
            var previous = this._current;
            this._current = thirdPersonControllerState;
            this._onChanged.Invoke(new ThirdPersonControllerStateEvent
            {
                fromState = previous,
                toState = this._current
            });
        }
    }
}