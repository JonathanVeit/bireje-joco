using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using BiReJeJoCo.Backend;

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
        AdvancedWalkerController walkController;

        //save active Axis numbers
        private float xAxisSave;
        private float yAxisSave;

        #region Initiazation
        protected override void OnSystemsInitialized()
        {
            cinemaFreeLook = this.GetComponent<CinemachineFreeLook>();

            //save axis values
            xAxisSave = cinemaFreeLook.m_XAxis.m_MaxSpeed;
            yAxisSave = cinemaFreeLook.m_YAxis.m_MaxSpeed;
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {  
            messageHub.RegisterReceiver<PauseMenuOpenedMsg>(this, HandleGameMenuOpened);
            messageHub.RegisterReceiver<PauseMenuClosedMsg>(this, HandleGameMenuClosed);
            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnMatchEnd);
        }
        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
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

        void HandleGameMenuOpened(PauseMenuOpenedMsg onGameMenuOpenedMsg)
        {
            BlockMovement();
        }
        void HandleGameMenuClosed(PauseMenuClosedMsg onGameMenuClosedMsg)
        {
            UnblockMovement();
        }

        void OnMatchEnd(PhotonMessage msg)
        {
            BlockMovement();
        }

        void BlockMovement()
        {
            xAxisSave = cinemaFreeLook.m_XAxis.m_MaxSpeed;
            yAxisSave = cinemaFreeLook.m_YAxis.m_MaxSpeed;
            cinemaFreeLook.m_XAxis.m_MaxSpeed = 0f;
            cinemaFreeLook.m_YAxis.m_MaxSpeed = 0f;
        }
        void UnblockMovement() 
        {
            cinemaFreeLook.m_XAxis.m_MaxSpeed = xAxisSave;
            cinemaFreeLook.m_YAxis.m_MaxSpeed = yAxisSave;
        }
    }
}
