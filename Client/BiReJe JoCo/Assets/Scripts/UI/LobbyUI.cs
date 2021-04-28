using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class LobbyUI : UIElement
    {
        [SerializeField] Text lobbyName;
        [SerializeField] UIList<LobbyMemberEntry> memberList;
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] Button startButton;

        private Dictionary<string, LobbyMemberEntry> memberEntries
            = new Dictionary<string, LobbyMemberEntry>();

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            startButton.gameObject.SetActive(localPlayer.IsHost);
            messageHub.RegisterReceiver<OnLoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        private void OnLobbySceneLoaded(OnLoadedLobbySceneMsg msg)
        {
            lobbyName.text = photonRoomWrapper.RoomName;
            foreach (var curPlayer in playerManager.GetAllPlayer())
                AddMemberListEntry(curPlayer);

            messageHub.UnregisterReceiver<OnLoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            ConnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<OnAddedPlayerMsg>(this, OnAddedPlayer);
            messageHub.RegisterReceiver<OnRemovedPlayerMsg>(this, OnRemovedPlayer);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, OnPrepareMatchStart);
        }

        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region UI 
        private void AddMemberListEntry(Player player)
        {
            var entry = memberList.Add();
            entry.Initialize(player.NickName);
            memberEntries.Add(player.NickName, entry);
        }

        private void RemoveMemberListEntry(Player player)
        {
            var entry = memberEntries[player.NickName];
            memberEntries.Remove(player.NickName);
            memberList.Remove(entry);
        }
        #endregion

        #region Events
        private void OnAddedPlayer(OnAddedPlayerMsg msg) 
        {
            AddMemberListEntry(msg.Param1);
        }

        private void OnRemovedPlayer(OnRemovedPlayerMsg msg)
        {
            RemoveMemberListEntry(msg.Param1);
        }

        private void OnLeftLobby(OnLeftLobbyMsg msg)
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
            (matchHandler as HostMatchHandler).StartMatch("game_scene");
        }

        public void LeaveLobby() 
        {
            photonClient.LeaveLobby();
        }
        #endregion
    }
}