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

        protected override void OnSystemsInitialized()
        {
            messageHub.RegisterReceiver<OnAddedPlayerMsg>(this, OnAddedPlayer);
            messageHub.RegisterReceiver<OnRemovedPlayerMsg>(this, OnRemovedPlayer);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);

            lobbyName.text = photonRoomWrapper.RoomName;
            foreach (var curPlayer in playerManager.GetAllPlayer())
                AddMemberListEntry(curPlayer);

            startButton.gameObject.SetActive(localPlayer.IsHost);
        }

        protected override void OnBeforeDestroy()
        {
            messageHub.UnregisterReceiver<OnAddedPlayerMsg>(this, OnAddedPlayer);
            messageHub.UnregisterReceiver<OnRemovedPlayerMsg>(this, OnRemovedPlayer);
            messageHub.UnregisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

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

        public void LeaveRoom() 
        {
            photonClient.LeaveLobby();
        }
        #endregion
    }
}