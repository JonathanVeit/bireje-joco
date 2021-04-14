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
        public event Action<DisconnectCause> OnDisconnectedFromPhoton;

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
        public bool IsInLobby { get; private set; }
        public bool IsInRoom { get; private set; }
        public string RoomName { get; private set; }

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonWrapper>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        #region Connect To Photon
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
            OnDisconnectedFromPhoton?.Invoke(cause);
            IsConnectedToMaster = false;
            IsConnectedToPhoton = false;
        }
        #endregion

        #region Connect To Master
        public override void OnConnectedToMaster()
        {
            OnConnectedToPhotonMaster?.Invoke();
            IsConnectedToMaster = true;
        }
        #endregion

        #region Connect To Lobby
        public void JoinLobby() 
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            OnJoinedPhotonLobby?.Invoke();
            IsInLobby = true;
        }

        public override void OnLeftLobby()
        {
            OnLeftPhotonLobby?.Invoke();
            IsInLobby = false;
        }
        #endregion

        #region Room Connection
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
            IsInRoom = true;
            RoomName = PhotonNetwork.CurrentRoom.Name;
            OnJoinedPhotonRoom?.Invoke(RoomName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            IsInRoom = false;
            RoomName = null;
            OnJoinPhotonRoomFailed?.Invoke(message);
        }

        public override void OnLeftRoom()
        {
            IsInRoom = false;
            RoomName = null;
            OnLeftPhotonRoom?.Invoke();
        }
        #endregion
    }
}