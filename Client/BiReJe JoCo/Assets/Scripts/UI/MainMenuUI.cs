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
            messageHub.RegisterReceiver<OnFailedToHostLobbyMsg>(this, OnHostLobbyFailed);
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinLobbyFailed);

            if (PlayerPrefs.HasKey("nickname"))
            {
                playerNickNameInput.text = PlayerPrefs.GetString("nickname");
                localPlayer.SetNickName(PlayerPrefs.GetString("nickname"));
            }
            else
            {
                playerNickNameInput.text = System.Guid.NewGuid().ToString();
                localPlayer.SetNickName(playerNickNameInput.text);
            }
        }

        protected override void OnBeforeDestroy()
        {
            messageHub.UnregisterReceiver<OnFailedToHostLobbyMsg>(this, OnHostLobbyFailed);
            messageHub.UnregisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.UnregisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinLobbyFailed);
        }

        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            gameManager.OpenLobby();
        }

        private void OnHostLobbyFailed(OnFailedToHostLobbyMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        private void OnJoinLobbyFailed(OnJoinLobbyFailedMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        #region UI Inputs
        public void SetNickName(string name) 
        {
            localPlayer.SetNickName(name);
            PlayerPrefs.SetString("nickname", name);
        }

        public void HostRoom()
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;
            photonClient.HostLobby(roomNameInput.text, 10);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void JoinRoom() 
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;

            photonClient.JoinLobby(roomNameInput.text);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void QuitGame() 
        {
            gameManager.Quit();
        }
        #endregion
    }
}