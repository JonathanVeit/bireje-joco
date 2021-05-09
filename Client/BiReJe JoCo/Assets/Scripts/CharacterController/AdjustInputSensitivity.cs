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
        [SerializeField] [Range(0f, 1f)] private float yAxisController;
        [SerializeField] [Range(0f, 1f)] private float xAxisController;

        [Space(10)]
        [SerializeField] CinemachineFreeLook cinemaFreeLook;
        [SerializeField] PlayerCharacterInput characterInput;

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
        }
        protected override void OnBeforeDestroy()
        {
        }
        #endregion

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
    }
}
