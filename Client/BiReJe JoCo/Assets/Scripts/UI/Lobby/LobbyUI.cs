using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class LobbyUI : UIElement
    {
        const string DURATION_KEY = "LastSetDuration";
        const string PREFERED_ROLE_KEY = "PreferedRole";

        [Header("Settings")]
        [SerializeField] UIList<LobbyMemberEntry> memberList;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] Button startButton;
        [SerializeField] Dropdown durationDropdown;
        [SerializeField] GameObject[] preferedRoleOutlines;
        [SerializeField] string matchMode = "default_match";

        private Dictionary<string, LobbyMemberEntry> memberEntries
            = new Dictionary<string, LobbyMemberEntry>();

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            startButton.gameObject.SetActive(localPlayer.IsHost);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            Cursor.lockState = CursorLockMode.Confined;

            durationDropdown.interactable = localPlayer.IsHost;
            if (PlayerPrefs.HasKey(DURATION_KEY))
            {
                durationDropdown.value = PlayerPrefs.GetInt(DURATION_KEY);
            }
            if (PlayerPrefs.HasKey(PREFERED_ROLE_KEY))
            {
                SetPlayerPreferedRole(PlayerPrefs.GetInt(PREFERED_ROLE_KEY));
            }
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        private void OnLobbySceneLoaded(LoadedLobbySceneMsg msg)
        {
            foreach (var curPlayer in playerManager.GetAllPlayer())
                AddMemberListEntry(curPlayer);

            messageHub.UnregisterReceiver<LoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            ConnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<AddedPlayerMsg>(this, OnAddedPlayer);
            messageHub.RegisterReceiver<RemovedPlayerMsg>(this, OnRemovedPlayer);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
            messageHub.RegisterReceiver<HostSwitchedMsg>(this, OnSwitchedHost);
            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, OnPrepareMatchStart);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);

            if (photonMessageHub != null)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region UI 
        private void AddMemberListEntry(Player player)
        {
            var entry = memberList.Add();
            entry.Initialize(player);
            memberEntries.Add(player.Id, entry);
        }

        private void RemoveMemberListEntry(Player player)
        {
            var entry = memberEntries[player.Id];
            memberEntries.Remove(player.Id);
            memberList.Remove(entry);
        }
        #endregion

        #region Events
        private void OnAddedPlayer(AddedPlayerMsg msg) 
        {
            AddMemberListEntry(msg.Param1);
        }
        private void OnRemovedPlayer(RemovedPlayerMsg msg)
        {
            RemoveMemberListEntry(msg.Param1);
        }
        private void OnSwitchedHost(HostSwitchedMsg msg)
        {
            LeaveLobby();
        }

        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            gameManager.OpenMainMenu();
        }

        private void OnPrepareMatchStart(PhotonMessage msg)
        {
            loadingOverlay.SetActive(true);
        }
        #endregion

        #region UI Inputs
        public void StartGame()
        {
            (matchHandler as HostMatchHandler).StartMatch(matchMode);
        }

        public void LeaveLobby() 
        {
            photonClient.LeaveLobby();
        }

        public void SetPlayerPreferedRole(int role)
        {
            switch (role)
            {
                case 0:
                    localPlayer.SetPreferedRole(PlayerRole.None);
                    break;
                case 1:
                    localPlayer.SetPreferedRole(PlayerRole.Hunter);
                    break;
                case 2:
                    localPlayer.SetPreferedRole(PlayerRole.Hunted);
                    break;
            }

            for (int i = 0; i < preferedRoleOutlines.Length; i++)
            {
                preferedRoleOutlines[i].SetActive(i==role);
            }
            PlayerPrefs.SetInt(PREFERED_ROLE_KEY, role);
        }

        public void SetPlayerReadToStart() 
        {
            localPlayer.SetReadyToStart(!localPlayer.ReadToStart);
        }
        #endregion
    }
}