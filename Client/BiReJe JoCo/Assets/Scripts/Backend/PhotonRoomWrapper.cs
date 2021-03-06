using UnityEngine;
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

        private const string HOST_NAME_KEY = "HN";
        private const string LOBBY_STATE_KEY = "LS";

        public bool IsInRoom { get; private set; }
        public string RoomName { get; private set; }
        public int PlayerCount { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 0; }
        public List<Photon.Realtime.Player> PlayerList { get => PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Players.Values.ToList() : null; }
        public List<RoomInfo> RoomList { get; private set; }
        public Room CurrentRoom => PhotonNetwork.CurrentRoom;

        private IMessageHub messageHub => DIContainer.GetImplementationFor<IMessageHub>();

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonRoomWrapper>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        // room list 
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            RoomList = roomList;
            messageHub.ShoutMessage(this, new PhotonRoomListUpdatedMsg(roomList.ToArray()));
        }
        
        // create room
        public void CreateRoom(string hostName, int maxPlayers = 1, bool isVisible = true, bool isOpen = true)
            {
                var properties = new ExitGames.Client.Photon.Hashtable();
                properties.Add(0, hostName);
                properties.Add(1, LobbyState.Open);
            
                var roomOptions = new RoomOptions()
                {
                    MaxPlayers = (byte)maxPlayers,
                    IsVisible = isVisible,
                    IsOpen = isOpen,
                    PublishUserId = true,
                    CustomRoomPropertiesForLobby = new string[2] { HOST_NAME_KEY, LOBBY_STATE_KEY },
                    CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() 
                    {
                        { HOST_NAME_KEY, hostName },
                        { LOBBY_STATE_KEY, LobbyState.Open },
                    }
                };

                PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString(), roomOptions);
            }
        public override void OnCreatedRoom()
        {
            messageHub.ShoutMessage(this, new LobbyCreatedMsg());
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            messageHub.ShoutMessage(this, new FailedToHostLobbyMsg(message));
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

            messageHub.ShoutMessage(this, new JoinedLobbyMsg(RoomName));
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            IsInRoom = false;
            RoomName = null;

            messageHub.ShoutMessage(this, new JoinLobbyFailedMsg(message));
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

            messageHub.ShoutMessage(this, new LeftLobbyMsg());
        }

        public void RemovePlayer(int playerNumber) 
        {
            PhotonNetwork.CloseConnection(CurrentRoom.GetPlayer(playerNumber));
        }

        // other player join / leave
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            messageHub.ShoutMessage(this, new PlayerJoinedLobbyMsg(newPlayer.UserId));
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            messageHub.ShoutMessage(this, new PlayerLeftLobbyMsg(otherPlayer.UserId));
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            messageHub.ShoutMessage(this, new HostSwitchedMsg());
        }

        // instantiation
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation, bool asRoomObject = false)
        {
            if (asRoomObject)
            {
                return PhotonNetwork.InstantiateRoomObject(prefabId, position, rotation);
            }

            return PhotonNetwork.Instantiate(prefabId, position, rotation);
        }

        public void Destroy(GameObject target)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}