using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEditor.VersionControl;

namespace BiReJeJoCo.Character
{
    public class PlayerCharacterInput : SystemBehaviour
    {
        bool characterInputIsActive = true;

        private Vector2 moveInput;
        private Vector2 lookInput;

        // Jump actions
        public event Action onJumpIsPressed;
        public event Action onJumpLetGo;

        //Sprint actions
        public event Action onSprintIsPressed;
        public event Action onSprintLetGo;

        //Menu
        public event Action onMenuPressed;


        //Thoughts
        //key action  .started is called 2 times // .performed called 1; .canceled

        protected override void OnSystemsInitialized()
        {
            //Register what to do when game menu is being opened
            messageHub.RegisterReceiver<OnGameMenuOpenedMsg>(this, HandleGameMenuOpened);

            //Register what to do when game menu being closed
            messageHub.RegisterReceiver<OnGameMenuClosedMsg>(this, HandleGameMenuClosed);
        }

        #region Set Input (PlayerInput Component)
        // assigns the new input system values to a vector and gives that vector back
        // call Player_Input.MovementInput to get a Vector3 
        public void SetMovementInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            Vector2 inputMovement = inputValue.ReadValue<Vector2>();
            moveInput = inputMovement;
        }

        public void SetLookInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            Vector2 inputMovement = inputValue.ReadValue<Vector2>();
            lookInput = inputMovement;
        }

        public void SetJumpInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            if (inputValue.performed)
            {
                onJumpIsPressed?.Invoke();
            }

            if (inputValue.canceled)
            {
                onJumpLetGo?.Invoke();
            }
        }

        public void SetSprintInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            if (inputValue.performed)
            {
                onSprintIsPressed?.Invoke();
            }

            if (inputValue.canceled)
            {
                onSprintLetGo?.Invoke();
            }
        }

        public void SetMenuInput(InputAction.CallbackContext inputValue)
        {
            if (inputValue.performed)
            {
                if (characterInputIsActive)
                {
                    //CharacterInput is being set inactive
                    messageHub.ShoutMessage(this, new OnGameMenuOpenedMsg());
                    
                }
                else
                {
                    //CharacterInput is being set active
                    messageHub.ShoutMessage(this, new OnGameMenuClosedMsg());
                }
            }
        }

        #endregion

        public void AdjustSensitivityController()
        {
            //TODO adjust camera sensitivity for controller
        }

        #region Get Movement Input
        public Vector2 GetMovementInput()
        {
            return moveInput;
        }

        public float GetHorizontalMovementInput()
        {
            return moveInput.x;
        }

        public float GetVerticalMovementInput()
        {
            return moveInput.y;
        }
		#endregion

		#region Get Look Input
		public Vector2 GetLookInput()
        {
            return lookInput;
        }
        public float GetHorizontalLookInput()
        {
            return lookInput.x;
        }

        public float GetVerticalLookInput()
        {
            return lookInput.y;
        }
        #endregion


        //Stop player actions when menu is open
        void HandleGameMenuOpened(OnGameMenuOpenedMsg onGameMenuOpenedMsg)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            characterInputIsActive = false;
        }

        void HandleGameMenuClosed(OnGameMenuClosedMsg onGameMenuClosedMsg)
        {
            characterInputIsActive = true;
        }
    }
}

