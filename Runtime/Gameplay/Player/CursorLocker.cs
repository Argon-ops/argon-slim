using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using UnityEngine;
using UnityEngine.Events;


namespace DuksGames.Argon.Gameplay
{
    public class CursorLocker : MonoBehaviour, ICursorLocker
    {
        public UnityEvent OnCursorStateChanged { get; private set; } = new UnityEvent();

        // Counter for the number of not-yet-released requests
        int claims = 0;

        [SerializeField] bool Ddisable;

        public void FreeCursor()
        {
            if (this.Ddisable) return;
            // if claims count is going from zero to one, it's time to unlock

            if (this.claims++ == 0)
            {
                CursorLockStateEnforcer.Suspend = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                OnCursorStateChanged.Invoke();
            }
        }

        public void LockCursor()
        {
            if (this.Ddisable) return;

            // if claims is about to become zero, it's time to lock
            if (this.claims == 1)
            {
                this.ForceLock();
                OnCursorStateChanged.Invoke();
            }

            this.claims = Mathf.Max(this.claims - 1, 0);
        }

        public void ForceLock()
        {
            if (this.Ddisable) return;

            CursorLockStateEnforcer.Suspend = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
}