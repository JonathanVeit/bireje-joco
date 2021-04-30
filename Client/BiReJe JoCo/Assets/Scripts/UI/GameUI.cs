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

        static bool isMenuActive = false;

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

            //Register what to do when game menu is being opened
            messageHub.RegisterReceiver<OnGameMenuOpenedMsg>(this, ToggleMenuOn);

            //Register what to do when game menu being closed
            messageHub.RegisterReceiver<OnGameMenuClosedMsg>(this, ToggleMenuOff);
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
            HandleGameMenuClosed();
        }

        public void EndMatch()
        {
            photonMessageHub.ShoutMessage(new QuitMatchPhoMsg(true), PhotonMessageTarget.AllViaServer);
        }

        public void SetMenuInput(InputAction.CallbackContext inputValue)
        {
            if (inputValue.performed)
            {
                if (!isMenuActive)
                {
                    HandleGameMenuOpened();
                }
                else
                {
                    HandleGameMenuClosed();
                }
            }
        }
        #endregion


        #region Toggle and Shout Menu and Messages
        void HandleGameMenuOpened()
        {
            messageHub.ShoutMessage(this, new OnGameMenuOpenedMsg());
            isMenuActive = true;
        }

        void HandleGameMenuClosed()
        {
            messageHub.ShoutMessage(this, new OnGameMenuClosedMsg());
            isMenuActive = false;
        }

        void ToggleMenuOn(OnGameMenuOpenedMsg onGameMenuOpenedMsg)
        {
            menuGO.SetActive(true);
        }

        void ToggleMenuOff(OnGameMenuClosedMsg onGameMenuOpenedMsg)
        {
            menuGO.SetActive(false);
        }
        #endregion
    }
}