using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;
using System.Collections;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        [Header("UI Elements")]
        public Transform floatingElementGrid;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] Text startInformation;
        [SerializeField] Text durationLabel;

        [Header("Hunter")]
        [SerializeField] GameObject hunterHUD;
        [SerializeField] GameObject crosshairGO;
        [SerializeField] UIBarHandler shootingCooldownBar;

        [Header("Hunted")]
        [SerializeField] GameObject huntedHUD;
        [SerializeField] UIBarHandler healthBar;
        [SerializeField] UIBarHandler transformationBar;
        [SerializeField] UIBarHandler transformationCooldownBar;

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
            if (localPlayer.Role == PlayerRole.Hunted)
            {
                InitializeAsHunted();
            }
            else if (localPlayer.Role == PlayerRole.Hunter)
            {
                InitializeAsHunter();
            }

            UpdateTransformationDurationBar(0, 1);
        }
        private void InitializeAsHunted()
        {
            hunterHUD.SetActive(false);

            startInformation.text = "You are the monster!\nTry to hide!";
            StartCoroutine(FadeText(3, startInformation));
        }
        private void InitializeAsHunter()
        {
            huntedHUD.SetActive(false);

            startInformation.text = "You are the Hunter!\nTry to kill the monster!";
            StartCoroutine(FadeText(3, startInformation));
        }

        private IEnumerator FadeText(float duration, Text target) 
        {
            float counter = duration;
            var color = target.color;
            while (counter >= 0)
            {
                color.a = counter / duration;
                target.color = color;

                counter -= Time.deltaTime;
                yield return null;
            }

            target.gameObject.SetActive(false);
        }
        public void UpdateWeaponCooldown(float value, float maxValue) 
        {
            if (value <= 0.1f)
                shootingCooldownBar.OverrideValue(0);
            else
                shootingCooldownBar.SetValue(value / maxValue);
        }

        public void UpdateDuration(string duration)
        {
            durationLabel.text = duration;
        }

        public void UpdateHealthBar(float value, float maxValue) 
        {
            healthBar.SetValue(value / maxValue);
        }
        public void UpdateTransformationDurationBar(float value, float maxValue)
        {
            if (value > 0)
            {
                transformationBar.TargetImage.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                transformationBar.TargetImage.transform.parent.gameObject.SetActive(false);
            }

            if (value / maxValue == 1)
                transformationBar.OverrideValue(1);
            else
                transformationBar.SetValue(value / maxValue);
        }
        public void UpdateTransformationCooldownBar(float value, float maxValue)
        {
            if (value == 0)
                transformationCooldownBar.OverrideValue(0);
            else
                transformationCooldownBar.SetValue(value / maxValue);
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
            resultPopup.Show(result);
            crosshairGO.SetActive(false);
        }
        #endregion
    }
}