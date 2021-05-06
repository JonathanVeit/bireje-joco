using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        [Header("UI Elements")]
        public Transform floatingElementGrid;
        [SerializeField] GameObject menuGO;
        [SerializeField] GameObject endMatchButton;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] GameObject crosshairGO;

        private MatchPausePopup pausePopup => uiManager.GetInstanceOf<MatchPausePopup>();
        private MatchResultPopup resultPopup => uiManager.GetInstanceOf<MatchResultPopup>();

        #region Inizialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DIContainer.RegisterImplementation<GameUI>(this);

            ConnectEvents();
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            endMatchButton.SetActive(localPlayer.IsHost);

            if (localPlayer.Role == PlayerRole.Hunted)
            {
                InitializeAsHunted();
            }
            else if (localPlayer.Role == PlayerRole.Hunter)
            {
                InitializeAsHunter();
            }
        }
        private void InitializeAsHunted()
        {
            crosshairGO.SetActive(false);
        }
        private void InitializeAsHunter() 
        {
            crosshairGO.SetActive(true);
        }
   

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PauseMenuOpenedMsg>(this, OnPauseMenuOpened);
            messageHub.RegisterReceiver<PauseMenuClosedMsg>(this, OnPauseMenuClosed);

            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnMatchStart);
            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnMatchFinished);
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);
        }
        private void DisconnectEvents()
        {
            photonMessageHub.UnregisterReceiver(this);
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        private void OnMatchStart(PhotonMessage msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }
        private void OnMatchFinished(PhotonMessage msg)
        {
            var casted = msg as FinishMatchPhoMsg;
            ShowResult(casted.result);
        }
        private void OnMatchClosed(PhotonMessage msg)
        {
            DIContainer.UnregisterImplementation<GameUI>();
            DisconnectEvents();
        }
        #endregion

        #region UI Input
        public void SetMenuInput(InputAction.CallbackContext inputValue)
        {
            if (!inputValue.performed) return;

            if (pausePopup.IsOpen)
            {
                messageHub.ShoutMessage<PauseMenuClosedMsg>(this);
            }
            else
            {
                messageHub.ShoutMessage<PauseMenuOpenedMsg>(this);
            }
        }
        #endregion

        #region Events
        void OnPauseMenuOpened(PauseMenuOpenedMsg onGameMenuOpenedMsg)
        {
            ToggleMenu();
        }
        void OnPauseMenuClosed(PauseMenuClosedMsg onGameMenuOpenedMsg)
        {
            ToggleMenu();
        }
        private void ToggleMenu() 
        {
            if (pausePopup.IsOpen)
                pausePopup.Hide();
            else
                pausePopup.Show();
        }

        private void ShowResult(MatchResult result) 
        {
            Cursor.lockState = CursorLockMode.Confined;
            resultPopup.Show(result);
            crosshairGO.SetActive(false);
        }
        #endregion
    }
}