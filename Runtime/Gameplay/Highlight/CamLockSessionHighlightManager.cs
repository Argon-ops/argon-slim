using UnityEngine;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    /// Handle object highlighting during cam lock sessions
    /// </summary>
    public class CamLockSessionHighlightManager : MonoBehaviour, ICamLockSessionOneAtATimeHighlightManager
    {
        IInteractionHighlight _current;

        public AudioSource _audioSource;

        void PlaySound()
        {
            this._audioSource?.Play();
        }

        public void NextHighlight(IInteractionHighlight next)
        {
            if (this._current == next)
            {
                return;
            }
            if (this._current != null)
            {
                this._current.Highlight(false);
                this._current = null;
            }
            if (next == null)
            {
                return;
            }
            this._current = next;
            this._current.Highlight(true);
            this.PlaySound();
        }
    }
}