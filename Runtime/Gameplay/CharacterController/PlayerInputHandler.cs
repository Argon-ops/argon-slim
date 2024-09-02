using System;
using DuksGames.Argon.Config;
using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Gameplay
{

    public class PlayerInputHandler : MonoBehaviour
    {


        class WeaponFireEnabler
        {
            public bool IsWeaponFireSuspended;
            public Func<bool> _IsWeaponFireClickEnabled { private get; set; }

            public bool IsWeaponFireClickEnabled()
            {
                if (this.IsWeaponFireSuspended) { return false; }
                return this._IsWeaponFireClickEnabled();
            }
        }

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 2.2f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        bool wasFireInputHeld;
        bool wasFireDownConsumed;

        WeaponFireEnabler fireEnabler;

        public bool IsInGameInputSuspended;

        void Start()
        {
            this.fireEnabler = new WeaponFireEnabler
            {
                _IsWeaponFireClickEnabled = () =>
                {
                    return this.GetAimInputHeld();
                },
            };


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        void LateUpdate()
        {
            wasFireInputHeld = GetFireInputHeld();
            this.wasFireDownConsumed = false;
        }


        public bool CanProcessInput()
        {
            // Assume that the cursor lock state is always a perfect proxy for whether 
            //   or not we should be able to move.
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                // developer-holding-sandwich mode: allow middle mouse move forward for those who are trying to play test and eat lunch at the same time
                if (Input.GetMouseButton(2))
                {
                    move.z = Input.GetMouseButton(1) ? -1f : 1f; // RMB down also moves backwards
                }

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal) * (InvertXAxis ? -1f : 1f);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical) * (InvertYAxis ? -1f : 1f);
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        #region weapon-fire

        public void SuspendWeaponFire(bool shouldSuspend)
        {
            this.fireEnabler.IsWeaponFireSuspended = shouldSuspend;
        }

        public bool GetItemInteractFireInputDown()
        {
            return !this.fireEnabler.IsWeaponFireClickEnabled() && this.GetFireInputDown();
        }

        public bool GetWeaponFireInputDown()
        {
            return this.fireEnabler.IsWeaponFireClickEnabled() && this.GetFireInputDown();
        }

        public bool GetWeaponFireInputReleased()
        {
            return this.fireEnabler.IsWeaponFireClickEnabled() && this.GetFireInputReleased();
        }

        public bool GetWeaponFireInputHeld()
        {
            return this.fireEnabler.IsWeaponFireClickEnabled() && this.GetFireInputHeld();
        }

        bool GetFireInputDown()
        {
            return !this.wasFireDownConsumed && GetFireInputHeld() && !wasFireInputHeld;
        }

        bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && wasFireInputHeld;
        }

        bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) != 0f;
                if (isGamepad)
                {
                    return Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) >= TriggerAxisThreshold;
                }
                return Input.GetButton(GameConstants.k_ButtonNameFire);
            }

            return false;
        }

        #endregion


        public bool GetAimInputHeld()
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) != 0f;
                bool i = isGamepad
                    ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) > 0f)
                    : Input.GetButton(GameConstants.k_ButtonNameAim);
                return i;
            }

            return false;
        }

        // public bool GetItemButtonDown() 
        // {
        //     if (CanProcessInput())
        //     {
        //         return this.aimButtonInput.WentDown();
        //     }
        //     return false;
        // }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameSprint);
            }
            return false;
        }

        public bool GetCrouchInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        public bool GetCrouchInputReleased()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        public bool GetReloadButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonReload);
            }

            return false;
        }

        public bool GetInspectButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonInspect);
            }
            return false;
        }

        public bool GetHolsterButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonHolster);
            }
            return false;
        }


        public int GetSelectWeaponInput()
        {
            if (CanProcessInput())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    return 1;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    return 2;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    return 3;
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    return 4;
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                    return 5;
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                    return 6;
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                    return 7;
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                    return 8;
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                    return 9;
                else
                    return 0;
            }

            return 0;
        }

        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
        {
            if (CanProcessInput())
            {
                // Check if this look input is coming from the mouse
                bool isGamepad = Input.GetAxis(stickInputName) != 0f;
                float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);
                
                // apply sensitivity multiplier
                i *= LookSensitivity;

                if (isGamepad)
                {
                    // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
                    i *= Time.deltaTime;
                }
                else
                {
                    // reduce mouse input amount to be equivalent to stick movement
                    i *= 0.01f;
#if UNITY_WEBGL
                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                    i *= WebglLookSensitivityMultiplier;
#endif
                }

                return i;
            }

            return 0f;
        }
    }
}