using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class MainMenuUI : UIElement
    {
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] InputField playerNickNameInput;
        [SerializeField] InputField roomNameInput;

        protected override void OnSystemsInitialized()
        {
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnRoomJoined);
            messageHub.RegisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinRoomFailed);

            playerNickNameInput.text = System.Guid.NewGuid().ToString();
        }

        protected override void OnBeforeDestroy()
        {
            messageHub.UnregisterReceiver<OnJoinedLobbyMsg>(this, OnRoomJoined);
            messageHub.UnregisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinRoomFailed);
        }

        private void OnRoomJoined(OnJoinedLobbyMsg msg)
        {
            gameManager.OpenLobby();
            loadingOverlay.gameObject.SetActive(false);
        }

        private void OnJoinRoomFailed(OnJoinLobbyFailedMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        #region UI Inputs
        public void SetNickName(string name) 
        {
            photonConnectionWrapper.NickName = name;
        }

        public void HostRoom()
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;
            photonClient.HostRoom(roomNameInput.text, 3);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void JoinRoom() 
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;

            photonClient.JoinRoom(roomNameInput.text);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void QuitGame() 
        {
            gameManager.Quit();
        }
        #endregion
    }
}