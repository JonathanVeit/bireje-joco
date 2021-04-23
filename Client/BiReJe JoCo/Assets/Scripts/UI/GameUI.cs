using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        public Transform floatingElementGrid;
        [SerializeField] GameObject menuGO;
        [SerializeField] GameObject endMatchButton;
        [SerializeField] GameObject loadingOverlay;

        #region Inizialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DIContainer.RegisterImplementation<GameUI>(this);
            endMatchButton.SetActive(localPlayer.IsHost);
            
            ConnectEvents();
        }
        
        private void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnMatchStart);
            photonMessageHub.RegisterReceiver<EndMatchPhoMsg>(this, OnFinishMatch);
            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);
        }

        private void DisconnectEvents()
        {
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        public override void Tick(float deltaTime)
        {
            if (Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            menuGO.SetActive(!menuGO.activeSelf);

            if (menuGO.activeSelf)
                messageHub.ShoutMessage(this, new OnGameMenuOpenedMsg());
            else
                messageHub.ShoutMessage(this, new OnGameMenuClosedMsg());
        }

        #region Events
        private void OnMatchStart(PhotonMessage msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        private void OnFinishMatch(PhotonMessage msg)
        { 
        }

        private void OnQuitMatch(PhotonMessage msg)
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
            photonMessageHub.ShoutMessage(new QuitMatchPhoMsg(true), PhotonMessageTarget.AllViaServer);
        }
        #endregion
    }
}