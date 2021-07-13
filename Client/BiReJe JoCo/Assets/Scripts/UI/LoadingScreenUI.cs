using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using UnityEngine.InputSystem;
using JoVei.Base.MessageSystem;
using System;

namespace BiReJeJoCo.UI
{
    public class LoadingScreenUI : UIElement
    {
        [Header("Settings")]
        [SerializeField] UIBarHandler loadingBar;
        [SerializeField] Text loadingBarCaption;
        [SerializeField] Text loadingBarProgressCaption;
        [SerializeField] GameObject loadingPanel;
        [SerializeField] GameObject titlePanel;
        [SerializeField] SimpleErrorPopup errorPopup;

        private void Start()
        {
            systemsLoader.OnSystemLoaded += OnSystemLoaded;
            systemsLoader.OnAllSystemsLoaded += OnAllSystemsLoaded;
            loadingBar.OverrideValue(0);
        }

        private void OnSystemLoaded(object system) 
        {
            loadingBar.SetValue(systemsLoader.CurrentProgress);
            loadingBarCaption.text = string.Format("{0} loaded...", system.GetType().Name);
            loadingBarProgressCaption.text = string.Format("{0}%", Mathf.RoundToInt(systemsLoader.CurrentProgress * 100));

            if (system is IMessageHub asMessageHub)
            {
                asMessageHub.RegisterReceiver<DisconnectedFromPhotonMsg>(this, OnFailedToBuildConnection);
            }
        }

        private void OnFailedToBuildConnection(DisconnectedFromPhotonMsg msg)
        {
            errorPopup.Show("CONNECTION ERROR",
                            "Failed to build connection.",
                            "Quit",
                            () =>
                            {
                                Application.Quit();
                            },
                            $"\"{ msg.Param1}\"",
                            "Retry?",
                            () =>
                            {
                                photonClient.BuildConnection();
                            }
                           );
        }

        private void OnAllSystemsLoaded() 
        {
            loadingBar.SetValue(1);
            loadingBarCaption.text = "finished...";
            loadingBarProgressCaption.text = "100%";

            loadingPanel.gameObject.SetActive(false);
            titlePanel.gameObject.SetActive(true);

            messageHub.UnregisterReceiver(this);
        }

        private void Update()
        {
            if (!systemsLoader.Finished)
                return;

            if (Keyboard.current.anyKey.wasPressedThisFrame) 
            {
                gameManager.OpenMainMenu();
            }
        }
        protected override void OnBeforeDestroy()
        {
            systemsLoader.OnStartLoadingSystem -= OnSystemLoaded;
            systemsLoader.OnAllSystemsLoaded -= OnAllSystemsLoaded;
        }
    }
}