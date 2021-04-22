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
            playerManager.onPlayerAdded += AddMemberListEntry;
            playerManager.onPlayerRemoved += RemoveMemberListEntry;

            lobbyName.text = photonRoomWrapper.RoomName;
            foreach (var curPlayer in playerManager.GetAllPlayer())
                AddMemberListEntry(curPlayer);

            startButton.gameObject.SetActive(localPlayer.IsHost);
        }

        protected override void OnBeforeDestroy()
        {
            playerManager.onPlayerAdded -= AddMemberListEntry;
            playerManager.onPlayerRemoved -= RemoveMemberListEntry;
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

        #region UI Inputs
        public void StartGame()
        {
            photonRoomWrapper.LoadLevel("game_scene");
        }

        public void LeaveRoom() 
        {
            photonClient.LeaveLobby();
            gameManager.OpenMainMenu();
        }
        #endregion
    }
}