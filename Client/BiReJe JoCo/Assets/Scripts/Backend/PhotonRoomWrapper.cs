using JoVei.Base;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

namespace BiReJeJoCo.Backend
{
    public class PhotonRoomWrapper : MonoBehaviourPunCallbacks, IInitializable
    {
        public event Action onRoomCreated;
        public event Action<string> onFailedToCreateRoom;

        public event Action<string> onJoinedRoom;
        public event Action<string> onJoinRoomFailed;
        public event Action onLeftRoom;

        public event Action<string> onPlayerEnteredRoom;
        public event Action<string> onPlayerLeftRoom;

        public bool IsInRoom { get; private set; }
        public bool IsHost { get => PhotonNetwork.LocalPlayer.IsMasterClient; }
        public string RoomName { get; private set; }
        public int PlayerCount { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 0; }
        public Dictionary<int, Player> PlayerList { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Players : null; }

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonRoomWrapper>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        // create room
        public void CreateRoom(string roomName, int maxPlayers = 1, bool isVisible = true, bool isOpen = true)
        {
            var roomOptions = new RoomOptions()
            {
                MaxPlayers = (byte)maxPlayers,
                IsVisible = isVisible,
                IsOpen = isOpen,
            };

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        public override void OnCreatedRoom()
        {
            onRoomCreated?.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            onFailedToCreateRoom?.Invoke(message);
        }


        // join room
        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnJoinedRoom()
        {
            IsInRoom = true;
            RoomName = PhotonNetwork.CurrentRoom.Name;
            onJoinedRoom?.Invoke(RoomName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            IsInRoom = false;
            RoomName = null;
            onJoinRoomFailed?.Invoke(message);
        }

        
        // leave room 
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            IsInRoom = false;
            RoomName = null;
            onLeftRoom?.Invoke();
        }

        
        // other player join / leave
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            onPlayerEnteredRoom?.Invoke(newPlayer.NickName);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            onPlayerLeftRoom?.Invoke(otherPlayer.NickName);
        }
    }
}