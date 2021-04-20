using JoVei.Base;
using JoVei.Base.MessageSystem;
using Photon.Pun;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

namespace BiReJeJoCo.Backend
{
    public class PhotonRoomWrapper : MonoBehaviourPunCallbacks, IInitializable
    {
        public bool IsInRoom { get; private set; }
        public bool IsHost { get => PhotonNetwork.LocalPlayer.IsMasterClient; }
        public string RoomName { get; private set; }
        public int PlayerCount { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 0; }
        public List<Photon.Realtime.Player> PlayerList { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Players.Values.ToList() : null; }

        private IMessageHub messageHub => DIContainer.GetImplementationFor<IMessageHub>();

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
                PublishUserId = true,
            };

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        public override void OnCreatedRoom()
        {
            messageHub.ShoutMessage(this, new OnLobbyCreatedMsg());
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            messageHub.ShoutMessage(this, new OnFailedToHostLobbyMsg(message));
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

            messageHub.ShoutMessage(this, new OnJoinedLobbyMsg(RoomName));
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            IsInRoom = false;
            RoomName = null;

            messageHub.ShoutMessage(this, new OnJoinLobbyFailedMsg(message));
        }

        // switch scene
        public void LoadLevel(string levelName) 
        {
            PhotonNetwork.LoadLevel(levelName);
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

            messageHub.ShoutMessage(this, new OnLeftLobbyMsg());
        }

        
        // other player join / leave
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            messageHub.ShoutMessage(this, new OnPlayerJoinedLobbyMsg(newPlayer.UserId));
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            messageHub.ShoutMessage(this, new OnPlayerLeftLobbyMsg(otherPlayer.UserId));
        }
    }
}