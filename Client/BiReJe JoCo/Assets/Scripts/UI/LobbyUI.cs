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
            ConnectEvents();
            lobbyName.text = photonRoomWrapper.RoomName;
            foreach (var curPlayer in playerManager.GetAllPlayer())
                AddMemberListEntry(curPlayer);

            startButton.gameObject.SetActive(localPlayer.IsHost);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<OnAddedPlayerMsg>(this, OnAddedPlayer);
            messageHub.RegisterReceiver<OnRemovedPlayerMsg>(this, OnRemovedPlayer);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
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
        #endregion

        #region UI Inputs
        public void StartGame()
        {
            photonRoomWrapper.LoadLevel("game_scene");
        }

        public void LeaveLobby() 
        {
            photonClient.LeaveLobby();
        }
        #endregion
    }
}