using JoVei.Base;
using Photon.Pun;
using System;
using System.Collections;
using Photon.Realtime;

namespace BiReJeJoCo
{
    public class PhotonWrapper : MonoBehaviourPunCallbacks, IInitializable
    {
        public event Action OnConnectedToPhoton;
        public event Action<string> OnDisconnectedFromPhoton;
        public event Action OnConnectedToPhotonMaster;

        public event Action OnJoinedPhotonLobby;
        public event Action OnLeftPhotonLobby;

        public event Action OnCreatedPhotonRoom;
        public event Action<string> OnFailedToCreatePhotonRoom;
        public event Action<string> OnJoinedPhotonRoom;
        public event Action<string> OnJoinPhotonRoomFailed;
        public event Action OnLeftPhotonRoom;

        public bool IsConnectedToPhoton { get; private set; }
        public bool IsConnectedToMaster { get; private set; }
        public bool IsInPhotonLobby { get; private set; }
        public bool IsInPhotonRoom { get; private set; }
        public string PhotonRoomName { get; private set; }

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonWrapper>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        #region Connection
        public void ConnectToPhoton ()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void DisconnectFromPhoton()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnected()
        {
            OnConnectedToPhoton?.Invoke();
            IsConnectedToPhoton = true;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            OnDisconnectedFromPhoton?.Invoke(cause.ToString());
            IsConnectedToMaster = false;
            IsConnectedToPhoton = false;
        }
    
        public override void OnConnectedToMaster()
        {
            OnConnectedToPhotonMaster?.Invoke();
            IsConnectedToMaster = true;
        }
        #endregion

        #region Lobby
        public void JoinLobby() 
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            OnJoinedPhotonLobby?.Invoke();
            IsInPhotonLobby = true;
        }

        public override void OnLeftLobby()
        {
            OnLeftPhotonLobby?.Invoke();
            IsInPhotonLobby = false;
        }
        #endregion

        #region Rooms
        public void CreateRoom(string roomName, int maxPlayers = 1, bool isVisible = true, bool isOpen = true)
        {
            var roomOptions = new RoomOptions()
            {
                MaxPlayers = (byte) maxPlayers,
                IsVisible = isVisible,
                IsOpen = isOpen,
            };

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnCreatedRoom()
        {
            OnCreatedPhotonRoom?.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            OnFailedToCreatePhotonRoom?.Invoke(message);
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnJoinedRoom()
        {
            IsInPhotonRoom = true;
            PhotonRoomName = PhotonNetwork.CurrentRoom.Name;
            OnJoinedPhotonRoom?.Invoke(PhotonRoomName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            IsInPhotonRoom = false;
            PhotonRoomName = null;
            OnJoinPhotonRoomFailed?.Invoke(message);
        }

        public override void OnLeftRoom()
        {
            IsInPhotonRoom = false;
            PhotonRoomName = null;
            OnLeftPhotonRoom?.Invoke();
        }
        #endregion
    }
}