using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class MainMenuUI : UIElement
    {
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] InputField playerNickNameInput;
        [SerializeField] InputField roomNameInput;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            ConnectEvents();

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
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<OnFailedToHostLobbyMsg>(this, OnHostLobbyFailed);
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<JoinLobbyFailedMsg>(this, OnJoinLobbyFailed);
        }

        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        private void OnHostLobbyFailed(OnFailedToHostLobbyMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            if (localPlayer.IsHost)
                gameManager.OpenLobby();
        }

        private void OnJoinLobbyFailed(JoinLobbyFailedMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        #region UI Inputs
        public void SetNickName(string name) 
        {
            localPlayer.SetNickName(name);
            PlayerPrefs.SetString("nickname", name);
        }

        public void HostLobby()
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;
            photonClient.HostLobby(roomNameInput.text, 10);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void JoinLobby() 
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