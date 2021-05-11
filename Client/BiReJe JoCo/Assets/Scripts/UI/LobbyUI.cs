using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class LobbyUI : UIElement
    {
        [Header("Settings")]
        [SerializeField] Text lobbyName;
        [SerializeField] UIList<LobbyMemberEntry> memberList;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] Button startButton;
        [SerializeField] Dropdown durationDropdown;

        private Dictionary<string, LobbyMemberEntry> memberEntries
            = new Dictionary<string, LobbyMemberEntry>();

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            startButton.gameObject.SetActive(localPlayer.IsHost);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            Cursor.lockState = CursorLockMode.Confined;

            durationDropdown.interactable = localPlayer.IsHost;
            SetMatchDuration(0);
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        private void OnLobbySceneLoaded(LoadedLobbySceneMsg msg)
        {
            lobbyName.text = photonRoomWrapper.RoomName;
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
            entry.Initialize(player.NickName, player.IsHost);
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

            if (msg.Param1.IsHost)
                photonClient.LeaveLobby();
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
            (matchHandler as HostMatchHandler).StartMatch("game_scene_4");
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
                    localPlayer.SetPreferedRole(PlayerRole.Hunter);
                    break;
                case 1:
                    localPlayer.SetPreferedRole(PlayerRole.Hunted);
                    break;
            }
        }

        public void SetMatchDuration(int duration)
        {
            if (!localPlayer.IsHost)
                return;

            switch (duration)
            {
                case 0:
                    (matchHandler as HostMatchHandler).SetDuration(5 * 60);
                    localPlayer.SetPreferedRole(PlayerRole.Hunter);
                    break;
                case 1:
                    (matchHandler as HostMatchHandler).SetDuration(10 * 60);
                    break;
                case 2:
                    (matchHandler as HostMatchHandler).SetDuration(15 * 60);
                    break;
                case 3:
                    (matchHandler as HostMatchHandler).SetDuration(20 * 60);
                    break;
            }
        }
        #endregion
    }
}