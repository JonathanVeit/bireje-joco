using UnityEngine;
using UnityEngine.InputSystem;
using System;

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
            //lock cursor
            Cursor.lockState = CursorLockMode.Locked;

            //Register what to do when game menu is being opened
            messageHub.RegisterReceiver<OnGameMenuOpenedMsg>(this, ReceiveGameMenuOpened);

            //Register what to do when game menu being closed
            messageHub.RegisterReceiver<OnGameMenuClosedMsg>(this, ReceiveGameMenuClosed);
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

        public void SetTriggerInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            if (inputValue.performed)
            {
                messageHub.ShoutMessage<OnPlayerPressedTriggerMsg>(this);
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

        
        void ReceiveGameMenuOpened(OnGameMenuOpenedMsg onGameMenuOpenedMsg)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            characterInputIsActive = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        void ReceiveGameMenuClosed(OnGameMenuClosedMsg onGameMenuOpenedMsg)
        {
            Cursor.lockState = CursorLockMode.Locked;
            characterInputIsActive = true;
        }
    }
}

