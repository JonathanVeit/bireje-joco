using UnityEngine;
using UnityEngine.InputSystem;
using JoVei.Base;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        public Transform floatingElementGrid;
        [SerializeField] GameObject menuGO;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DIContainer.RegisterImplementation<GameUI>(this);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DIContainer.UnregisterImplementation<GameUI>();
        }

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
        private void OnLeftLobby(OnLeftLobbyMsg msg)
        {
            messageHub.UnregisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
            gameManager.OpenMainMenu();
        }
        #endregion

        #region UI Input
        public void Continue()
        {
            ToggleMenu();
        }

        public void Leave() 
        {
            photonRoomWrapper.LeaveRoom();
        }
        #endregion
    }
}