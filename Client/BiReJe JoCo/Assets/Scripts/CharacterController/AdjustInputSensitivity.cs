using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace BiReJeJoCo.Character
{
    public class AdjustInputSensitivity : TickBehaviour
    {
        [Header("Keyboard Cam")]
        [SerializeField] [Range(0f, 1f)] private float yAxisKeyboard;
        [SerializeField] [Range(0f, 1f)] private float xAxisKeyboard;

        [Header("Controller Cam")]
        [SerializeField] [Range(0f, 0.5f)] private float yAxisController;
        [SerializeField] [Range(0f, 6f)] private float xAxisController;

        [Space(10)]
        [Header("Cinemamachine Components")]
        [SerializeField] CinemachineVirtualCamera cinemaVirtual;
        [SerializeField] CinemachineFreeLook cinemaFreeLook;

        PlayerCharacterInput characterInput => localPlayer.PlayerCharacter.controllerSetup.characterInput;
        PlayerInput playerInput => localPlayer.PlayerCharacter.controllerSetup.playerInput;

        string currentControlScheme;

        //save active Axis numbers
        private float xAxisSave;
        private float yAxisSave;
        private bool wasBlocked;

        #region Initiazation
        protected override void OnSystemsInitialized()
        {
            characterInput.onBlockStateChanged += OnBlockStateChanged;

            //save axis values
            xAxisSave = cinemaFreeLook.m_XAxis.m_MaxSpeed;
            yAxisSave = cinemaFreeLook.m_YAxis.m_MaxSpeed;

            currentControlScheme = playerInput.currentControlScheme;
        }
        protected override void OnBeforeDestroy()
        {
        }
        #endregion

        
        private void Update()
        {
            if (playerInput.currentControlScheme != currentControlScheme)
            {
                OnControlsChanged(playerInput);
                currentControlScheme = playerInput.currentControlScheme;
            }
        }

        public void OnControlsChanged(PlayerInput playerInput)
        {
            if (playerInput.currentControlScheme == "Keyboard")
            {
                cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisKeyboard;
                cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisKeyboard;
            }
            else if (playerInput.currentControlScheme == "Gamepad")
            {
                cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisController;
                cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisController;
            }
        }

        private void OnBlockStateChanged(InputBlockState state)
        {
            if (cinemaFreeLook)
            {
                if (state.HasFlag(InputBlockState.Look))
                {
                    xAxisSave = cinemaFreeLook.m_XAxis.m_MaxSpeed;
                    yAxisSave = cinemaFreeLook.m_YAxis.m_MaxSpeed;
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = 0f;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = 0f;
                    wasBlocked = true;
                }
                else if (wasBlocked)
                {
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisSave;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisSave;
                    wasBlocked = false;
                }
            }
            else if (cinemaVirtual)
            {
                if (state.HasFlag(InputBlockState.Look))
                {
                    //xAxisSave = cinemaVirtual.
                    //yAxisSave = cinemaVirtual.m_YAxis.m_MaxSpeed;
                    //cinemaVirtual.m_XAxis.m_MaxSpeed = 0f;
                    //cinemaFreeLook.m_YAxis.m_MaxSpeed = 0f;
                    //wasBlocked = true;
                }
                else if (wasBlocked)
                {
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisSave;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisSave;
                    wasBlocked = false;
                }
            }
        }
    }
}
