using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class MainMenuUI : UIElement
    {
        [Header("Settings")]
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] InputField playerNickNameInput;
        [SerializeField] UIList<LobbyListEntry> lobbyList;

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

            Cursor.lockState = CursorLockMode.Confined;

            UpdateLobbyList(lobbyManager.GetOpenLobbies());
        }

        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<LobbyListUpdatedMsg>(this, OnLobbyListUpdated);
            messageHub.RegisterReceiver<FailedToHostLobbyMsg>(this, OnHostLobbyFailed);
            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<JoinLobbyFailedMsg>(this, OnJoinLobbyFailed);
        }

        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        private void OnLobbyListUpdated(LobbyListUpdatedMsg msg)
        {
            UpdateLobbyList(msg.lobbies);
        }

        private void OnHostLobbyFailed(FailedToHostLobbyMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }
        
        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            if (localPlayer.IsHost)
                gameManager.OpenLobby();
        }
        private void OnJoinLobbyFailed(JoinLobbyFailedMsg msg)
        {
            loadingOverlay.gameObject.SetActive(false);
        }
        #endregion

        #region UI Inputs
        public void UpdateLobbyList(LobbyInfo[] lobbies)
        {
            lobbyList.Clear();

            foreach (var lobby in lobbies)
            {
                if (lobby.PlayerAmount == 0) continue;

                var entry = lobbyList.Add();
                entry.Initialize(lobby);
            }
        }

        public void SetNickName(string name) 
        {
            localPlayer.SetNickName(name);
            PlayerPrefs.SetString("nickname", name);
        }

        public void HostLobby()
        {
            photonClient.HostLobby(5);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void JoinLobby(string lobbyId) 
        {
            if (string.IsNullOrEmpty(lobbyId)) return;

            photonClient.JoinLobby(lobbyId);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void QuitGame() 
        {
            gameManager.Quit();
        }
        #endregion
    }
}