using UnityEngine;
using UnityEngine.InputSystem;

namespace BiReJeJoCo.UI
{
    public class GameUI : UIElement
    {
        [SerializeField] GameObject menuGO;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
        }

        protected override void OnBeforeDestroy()
        {
            messageHub.UnregisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
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
                messageHub.ShoutMessage<OnGameMenuOpenedMsg>(this, new OnGameMenuOpenedMsg());
            else
                messageHub.ShoutMessage<OnGameMenuClosedMsg>(this, new OnGameMenuClosedMsg());
        }

        #region Events
        private void OnLeftLobby(OnLeftLobbyMsg msg)
        {
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