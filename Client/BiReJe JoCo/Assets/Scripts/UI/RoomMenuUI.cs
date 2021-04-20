using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class RoomMenuUI : UIElement
    {
        [SerializeField] Text roomName;
        [SerializeField] UIList<RoomMemberEntry> roomMemberList;
        [SerializeField] GameObject loadingOverlay;

        private Dictionary<string, RoomMemberEntry> memberEntries
            = new Dictionary<string, RoomMemberEntry>();

        protected override void OnSystemsInitialized()
        {
            photonRoomWrapper.onPlayerEnteredRoom += AddMember;
            photonRoomWrapper.onPlayerLeftRoom += RemoveMember;

            roomName.text = photonRoomWrapper.RoomName;
            foreach (var curPlayer in photonRoomWrapper.PlayerList.Values)
                AddMember(curPlayer.NickName);

            photonMessageHub.RegisterReceiver<PhotonTestMessage>(this, OnTestMessageReceived);
            photonMessageHub.ShoutMessage(new PhotonTestMessage(), PhotonMessageTarget.All);
        }

        private void OnTestMessageReceived(PhotonMessage message) 
        {
        }

        protected override void OnBeforeDestroy()
        {
            photonRoomWrapper.onPlayerEnteredRoom -= AddMember;
            photonRoomWrapper.onPlayerLeftRoom -= RemoveMember;
        }

        private void AddMember(string playerName) 
        {
            var entry = roomMemberList.Add();
            entry.Initialize(playerName);
            memberEntries.Add(playerName, entry);
        }

        private void RemoveMember(string playerName)
        {
            var entry = memberEntries[playerName];
            memberEntries.Remove(playerName);
            roomMemberList.Remove(entry);
        }

        #region UI Inputs

        public void LeaveRoom() 
        {
            photonClient.LeaveRoom();
            gameManager.OpenMainMenu();
        }
        #endregion
    }
}