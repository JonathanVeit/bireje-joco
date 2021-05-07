using UnityEngine;
using UnityEngine.InputSystem;
using System;
using BiReJeJoCo.Backend;

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

        // shooting 
        public event Action onShootPressed;

        //Thoughts
        //key action  .started is called 2 times // .performed called 1; .canceled

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            //lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PauseMenuOpenedMsg>(this, OnPauseMenuOpened);
            messageHub.RegisterReceiver<PauseMenuClosedMsg>(this, OnPauseMenuClosed);

            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnFinishMatch);
        }
        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

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

        public void SetShootInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            if (inputValue.performed)
            {
                onShootPressed?.Invoke();
            }
        }
        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                onShootPressed?.Invoke();
            }
        }

        public void SetTriggerInput(InputAction.CallbackContext inputValue)
        {
            if (!characterInputIsActive)
                return;

            if (inputValue.performed)
            {
                messageHub.ShoutMessage<PlayerPressedTriggerMsg>(this);
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

        #region Events
        void OnPauseMenuOpened(PauseMenuOpenedMsg msg)
        {
            BlockMovement();
            Cursor.lockState = CursorLockMode.Confined;
        }
        void OnPauseMenuClosed(PauseMenuClosedMsg msg)
        {
            UnblockMovement();
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnFinishMatch(PhotonMessage msg)
        {
            BlockMovement();
        }
        #endregion

        private void BlockMovement()
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            characterInputIsActive = false;
        }

        private void UnblockMovement() 
        {
            characterInputIsActive = true;
        }
    }
}

