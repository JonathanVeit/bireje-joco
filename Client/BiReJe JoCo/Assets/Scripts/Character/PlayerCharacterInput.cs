using UnityEngine;
using UnityEngine.InputSystem;
using System;
using BiReJeJoCo.Backend;
using System.Collections;

namespace BiReJeJoCo.Character
{
    public class PlayerCharacterInput : SystemBehaviour
    {
        public InputBlockState BlockState { get; private set; }
            = InputBlockState.Free;
        public event Action<InputBlockState> onBlockStateChanged;

        public PlayerInput playerInput;
        public Tp_Character_Input_Actions playerControlsAsset;

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

        // trigger 
        public event Action onTriggerPressed;
        public event Action<float> onTriggerHold;
        public event Action onTriggerReleased;
        private Coroutine onTriggerHoldInvoker;

        // shooting 
        public event Action onShootPressed;
        public event Action<float> onShootHold;
        public event Action onShootReleased;
        private Coroutine onShootHoldInvoker;

        // reloading 
        public event Action onReloadPressed;

        // throw trap
        public event Action onThrowTrapPressed;

        // flashlight
        public event Action onToggleFlashlightPressed;

        // spawning ping 
        public event Action onSpawnPingPressed;

        // speed up
        public event Action onSpeedUpPressed;

        // spawn corals
        public event Action onSpawnCoralsPressed;

        //Thoughts
        //key action  .started is called 2 times // .performed called 1; .canceled

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            //lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            playerControlsAsset = new Tp_Character_Input_Actions();
            playerControlsAsset.Enable();
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<BlockPlayerControlsMsg>(this, BlockPlayerControls);
            messageHub.RegisterReceiver<UnblockPlayerControlsMsg>(this, UnblockPlayerControls);

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
            if (BlockState.HasFlag(InputBlockState.Movement))
                return;

            Vector2 inputMovement = inputValue.ReadValue<Vector2>();
            moveInput = inputMovement;
        }

        public void SetLookInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Look))
                return;

            Vector2 inputLook = inputValue.ReadValue<Vector2>();
            lookInput = inputLook * optionManager.Sensitivity;
        }

        public void SetJumpInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Movement))
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
            if (BlockState.HasFlag(InputBlockState.Movement))
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
            if (BlockState.HasFlag(InputBlockState.Shoot))
                return;

            if (inputValue.performed)
            {
                onShootPressed?.Invoke();
                onShootHoldInvoker = StartCoroutine(OnShootHoldInvoker());
            }
            else if (inputValue.canceled)
            {
                onShootReleased?.Invoke();
                if (onShootHoldInvoker != null)
                    StopCoroutine(onShootHoldInvoker);
            }
        }
        private IEnumerator OnShootHoldInvoker()
        {
            float duration = 0;

            while (true)
            {
                onShootHold?.Invoke(duration);
                duration += Time.deltaTime;
                yield return null;
            }
        }

        public void SetReloadInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Interact))
                return;

            if (inputValue.performed)
            {
                onReloadPressed?.Invoke();
            }
        }

        public void SetThrowTrapInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Interact))
                return;

            if (inputValue.performed)
            {
                onThrowTrapPressed?.Invoke();
            }
        }

        public void SetToggleFlashlight(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Interact))
                return;

            if (inputValue.performed)
            {
                onToggleFlashlightPressed?.Invoke();
            }
        }

        public void SetTriggerInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Interact))
                return;

            if (inputValue.performed)
            {
                onTriggerPressed?.Invoke();
                onTriggerHoldInvoker = StartCoroutine(OnTriggerHoldInvoker());
            }
            else if (inputValue.canceled)
            {
                onTriggerReleased?.Invoke();
                if (onTriggerHoldInvoker != null)
                    StopCoroutine(onTriggerHoldInvoker);
            }
        }
        private IEnumerator OnTriggerHoldInvoker() 
        {
            float duration = 0;

            while (true)
            {
                onTriggerHold?.Invoke(duration);
                duration += Time.deltaTime;
                yield return null;
            }
        }

        public void SetSpawnPingInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Interact))
                return;
            if (inputValue.performed)
            {
                onSpawnPingPressed?.Invoke();
            }
        }

        public void SetSpeedUpPressed(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Shoot))
                return;

            if (inputValue.performed)
            {
                onSpeedUpPressed?.Invoke();
            }
        }
        public void SetSpawnCoralsInput(InputAction.CallbackContext inputValue)
        {
            if (BlockState.HasFlag(InputBlockState.Shoot))
                return;

            if (inputValue.performed)
            {
                onSpawnCoralsPressed?.Invoke();
            }
        }
        
        private void Update()
        {
            if (BlockState.HasFlag(InputBlockState.Shoot))
                return;
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                onShootPressed?.Invoke();
                onShootHoldInvoker = StartCoroutine(OnShootHoldInvoker());
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                onShootReleased?.Invoke();
                if (onShootHoldInvoker != null)
                    StopCoroutine(onShootHoldInvoker);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                onSpawnPingPressed?.Invoke();
                onSpawnCoralsPressed?.Invoke();
            }

            if (BlockState.HasFlag(InputBlockState.Look))
                return;
            
            if (playerInput.currentControlScheme == "Keyboard")
            {
                lookInput = playerControlsAsset.Player.LookAround.ReadValue<Vector2>() * optionManager.Sensitivity;
            }
        }
        #endregion

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
        void BlockPlayerControls(BlockPlayerControlsMsg msg)
        {
            SetBlockState(msg.blockState);

            if (BlockState.HasFlag(InputBlockState.Movement))
                moveInput = Vector2.zero;

            if (BlockState.HasFlag(InputBlockState.Look))
                lookInput = Vector2.zero;
        }
        void UnblockPlayerControls(UnblockPlayerControlsMsg msg)
        {
            SetBlockState(msg.blockState);
        }

        void OnFinishMatch(PhotonMessage msg)
        {
            SetBlockState(InputBlockState.Menu);
        }
        #endregion

        private void SetBlockState(InputBlockState state) 
        {
            BlockState = state;
            onBlockStateChanged?.Invoke(state);
        }
    }
}