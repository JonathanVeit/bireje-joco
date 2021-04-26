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

        protected override void OnSystemsInitialized()
        {
            cinemaFreeLook = this.GetComponent<CinemachineFreeLook>();
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

    }
}
