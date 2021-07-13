using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

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

        PlayerCharacterInput characterInput => localPlayer.PlayerCharacter.ControllerSetup.CharacterInput;
        PlayerInput playerInput => localPlayer.PlayerCharacter.ControllerSetup.PlayerInput;

        //save active Axis numbers
        private float SensitivityX
        {
            get
            {
                if (playerInput.currentControlScheme == "Keyboard")
                    return xAxisKeyboard * optionManager.Sensitivity;

                return xAxisController * optionManager.Sensitivity;
            }
        }
        private float SensitivityY 
        { 
            get 
            {
                if (playerInput.currentControlScheme == "Keyboard")
                    return yAxisKeyboard * optionManager.Sensitivity;

                return yAxisController * optionManager.Sensitivity;
            }
        }

        private bool isBlocked;

        #region Initiazation
        protected override void OnSystemsInitialized()
        {
            characterInput.onBlockStateChanged += OnBlockStateChanged;

            cinemaFreeLook.m_XAxis.m_MaxSpeed = SensitivityX;
            cinemaFreeLook.m_YAxis.m_MaxSpeed = SensitivityY;
        }

        protected override void OnBeforeDestroy()
        {
            characterInput.onBlockStateChanged -= OnBlockStateChanged;
        }
        #endregion

        private void OnBlockStateChanged(InputBlockState state)
        {
            if (cinemaFreeLook)
            {
                if (state.HasFlag(InputBlockState.Look))
                {
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = 0f;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = 0f;
                    isBlocked = true;
                }
                else if (isBlocked)
                {
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = SensitivityX;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = SensitivityY;
                    isBlocked = false;
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
                else if (isBlocked)
                {
                    cinemaFreeLook.m_XAxis.m_MaxSpeed = SensitivityX;
                    cinemaFreeLook.m_YAxis.m_MaxSpeed = SensitivityY;
                    isBlocked = false;
                }
            }
        }
    }
}
