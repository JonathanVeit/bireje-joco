using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;



namespace BiReJeJoCo.Character
{
    public class AdjustInputSensitivity : SystemBehaviour
    {
        [Header("Keyboard Cam")]
        [SerializeField] [Range(0f, 1f)] private float yAxisKeyboard;
        [SerializeField] [Range(0f,1f)]private float xAxisKeyboard;
        
        [Header("Controller Cam")]
        [SerializeField] [Range(0f, 1f)] private float yAxisController;
        [SerializeField] [Range(0f, 1f)] private float xAxisController;

        CinemachineFreeLook cinemaFreeLook;

        //save active Axis numbers
        private float xAxisSave;
        private float yAxisSave;

        protected override void OnSystemsInitialized()
        {
            cinemaFreeLook = this.GetComponent<CinemachineFreeLook>();

            //Register what to do when game menu is being opened
            messageHub.RegisterReceiver<OnGameMenuOpenedMsg>(this, HandleGameMenuOpened);

            //Register what to do when game menu being closed
            messageHub.RegisterReceiver<OnGameMenuClosedMsg>(this, HandleGameMenuClosed);
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


        void HandleGameMenuOpened(OnGameMenuOpenedMsg onGameMenuOpenedMsg)
        {
            xAxisSave = cinemaFreeLook.m_XAxis.m_MaxSpeed;
            yAxisSave = cinemaFreeLook.m_YAxis.m_MaxSpeed;
            cinemaFreeLook.m_XAxis.m_MaxSpeed = 0f;
            cinemaFreeLook.m_YAxis.m_MaxSpeed = 0f;
        }

        void HandleGameMenuClosed(OnGameMenuClosedMsg onGameMenuClosedMsg)
        {
            cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisSave;
            cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisSave;
        }

    }
}
