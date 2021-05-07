using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        [Header("UI Elements")]
        public Transform floatingElementGrid;
        [SerializeField] GameObject endMatchButton;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] GameObject crosshairGO;
        [SerializeField] UIBarHandler healthBar;
        [SerializeField] UIBarHandler bulletCooldownBar;

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

        #region UI 
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
            bulletCooldownBar.TargetImage.transform.parent.gameObject.SetActive(false);
        }
        private void InitializeAsHunter()
        {
            crosshairGO.SetActive(true);
            healthBar.TargetImage.transform.parent.gameObject.SetActive(false);
        }

        public void UpdateHealthBar(float value, float maxValue) 
        {
            healthBar.SetValue(value / maxValue);
        }

        public void UpdateCooldown(float value, float maxValue) 
        {
            if (value <= 0.1f)
                bulletCooldownBar.OverrideValue(0);
            else
                bulletCooldownBar.SetValue(value / maxValue);
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