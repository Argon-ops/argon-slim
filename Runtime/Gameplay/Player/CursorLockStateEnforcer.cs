using UnityEngine;
using DuksGames.Argon.Config;

namespace DuksGames.Argon.Gameplay
{
    public static class CursorLockStateEnforcer
    {

        public static bool Suspend;

        public static void UpdateCursorLock()
        {
            if (CursorLockStateEnforcer.Suspend)
                return;
            // Unsophisticated cursor lock state handling. Anything that needs to can suspend cursor lock enforcement.
            //   We really hope that no two processes decide to suspend-reenstate enforcement in an overlapping fashion.... (no this is problematic
            //     imagine someone looking at a desk top and then they want to pause). 
            //      What we want is a stack of callback functions. where this function is the fallback. always the last callback on the stack

            //  NOTE: there's a bug where the cursor never locks in the editor if the Game window is docked
            //    and mode is 'Play Maximized'  https://forum.unity.com/threads/cursor-stop-hiding-in-play-maximized.1544189/
            //     work-around is to undock it (and put it in our other monitor) (Or just use Play Focused mode)

            if (Cursor.lockState != CursorLockMode.None && Input.GetButtonDown(GameConstants.k_ButtonNameCancel))
            {
                // for now : let escape lift the cursor lock state
                //   later: TODO: decide how to implement cursor / camera locking
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            // if unlocked, check if they click
            if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}