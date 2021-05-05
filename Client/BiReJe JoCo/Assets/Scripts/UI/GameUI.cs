using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine.UI;

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
        [SerializeField] MatchResultPopup resultPopup;

        bool menuIsActive => menuGO.activeSelf;

        #region Inizialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DIContainer.RegisterImplementation<GameUI>(this);
            endMatchButton.SetActive(localPlayer.IsHost);

            ConnectEvents();
            InitializeUI();
        }
        private void InitializeUI()
        {
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
        public void Continue()
        {
            ToggleMenu();
        }

        public void EndMatch()
        {
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.LeaveLobby);
        }

        public void SetMenuInput(InputAction.CallbackContext inputValue)
        {
            if (!inputValue.performed) return;

            if (!menuIsActive)
            {
                messageHub.ShoutMessage(this, new PauseMenuOpenedMsg());
            }
            else
            {
                messageHub.ShoutMessage(this, new PauseMenuClosedMsg());
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
            menuGO.SetActive(!menuGO.activeSelf);
        }

        private void ShowResult(MatchResult result) 
        {
            Cursor.lockState = CursorLockMode.Confined;
            uiManager.GetInstanceOf<MatchResultPopup>().Show(result);
            crosshairGO.SetActive(false);
        }
        #endregion
    }
}