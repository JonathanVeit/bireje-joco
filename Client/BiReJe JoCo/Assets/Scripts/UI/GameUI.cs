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
        [SerializeField] UIBarHandler ammoBar;
        [SerializeField] UIBarHandler pingCooldownBar;

        [Header("Hunted")]
        [SerializeField] GameObject huntedHUD;
        [SerializeField] UIBarHandler healthBar;
        [SerializeField] UIBarHandler transformationBar;
        [SerializeField] UIBarHandler transformationCooldownBar;
        [SerializeField] UIBarHandler speedUpBar;
        [SerializeField] Image scannedItemIcon;
        [SerializeField] Text collectableLabel;
        [SerializeField] Image hitOverlay;

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

            UpdateTransformationDurationBar(0);
        }
        private void InitializeAsHunted()
        {
            hunterHUD.SetActive(false);
            UpdateCollectedItemAmount(0);
            startInformation.text = "You are the monster!\nTry to hide!";
            UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey("empty"));
            StartCoroutine(FadeText(3, startInformation));
        }
        private void InitializeAsHunter()
        {
            huntedHUD.SetActive(false);

            startInformation.text = "You are the Hunter!\nTry to kill the monster!";
            StartCoroutine(FadeText(3, startInformation));
        }

        public void UpdateMatchDuration(string duration)
        {
            durationLabel.text = duration;
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

        public void UpdateAmmoBar(float value)
        {
            if (value == 0)
                ammoBar.OverrideValue(0);
            else
                ammoBar.SetValue(value);
        }
        public void UpdatePingCooldown(float value)
        {
            if (value == 0f)
                pingCooldownBar.OverrideValue(0);
            else
                pingCooldownBar.SetValue(value);
        }

        public void UpdateHealthBar(float value)
        {
            healthBar.SetValue(value);
        }
        public void UpdateTransformationDurationBar(float value)
        {
            if (value > 0)
            {
                transformationBar.TargetImage.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                transformationBar.TargetImage.transform.parent.gameObject.SetActive(false);
            }

            if (value == 1)
                transformationBar.OverrideValue(1);
            else
                transformationBar.SetValue(value);
        }
        public void UpdateTransformationCooldownBar(float value)
        {
            if (value == 0)
                transformationCooldownBar.OverrideValue(0);
            else
                transformationCooldownBar.SetValue(value);
        }
        public void UpdateScannedItemIcon(Sprite icon)
        {
            scannedItemIcon.sprite = icon;
        }
        public void UpdateCollectedItemAmount(int amount)
        {
            collectableLabel.text = string.Format("{0} / {1}", amount, matchHandler.MatchConfig.Mode.huntedCollectables);
        }
        public void UpdateSpeedUpBar(float value)
        {
            if (value == 0)
                speedUpBar.OverrideValue(value);
            else
                speedUpBar.SetValue(value);
        }
        public void UpdateHitOverlay(float alpha)
        {
            var col = hitOverlay.color;
            col.a = alpha;
            hitOverlay.color = col;
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