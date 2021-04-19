using System.Collections;
using UnityEngine;
using JoVei.Base;
using JoVei.Base.Helper;

namespace BiReJeJoCo.Backend
{
    /// <summary>
    /// Photon client should be responsible for building the connection, hosting and joining rooms
    /// </summary>
    public class PhotonClient : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonClient>(this);
            ConnectEvents();

            BuildConnection();
            yield return new WaitUntil(() => photonConnectionWrapper.IsInLobby);
        }

        public void CleanUp()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            photonConnectionWrapper.onConnectedToMaster += OnConnectedToMaster;
            photonConnectionWrapper.onDisconnected += OnDisconnected;
            photonConnectionWrapper.onJoinedLobby += OnJoinedLobby;
            photonConnectionWrapper.onLeftLobby += OnLeftLobby;

            photonRoomWrapper.onJoinedRoom += OnRoomJoined;
            photonRoomWrapper.onJoinRoomFailed += OnJoinRoomFailed;
            photonRoomWrapper.onLeftRoom += OnLeftRoom;
        }

        private void DisconnectEvents()
        {
            photonConnectionWrapper.onConnectedToMaster -= OnConnectedToMaster;
            photonConnectionWrapper.onDisconnected -= OnDisconnected;
            photonConnectionWrapper.onJoinedLobby -= OnJoinedLobby;
            photonConnectionWrapper.onLeftLobby -= OnLeftLobby;

            photonRoomWrapper.onJoinedRoom -= OnRoomJoined;
            photonRoomWrapper.onJoinRoomFailed -= OnJoinRoomFailed;
            photonRoomWrapper.onLeftRoom -= OnLeftRoom;
        }
        #endregion

        #region Connection
        public void BuildConnection()
        {
            DebugHelper.PrintFormatted("Building connecting to photon.");
            photonConnectionWrapper.Connect();
        }

        private void OnConnectedToMaster()
        {
            DebugHelper.PrintFormatted("<color=green>Successfully connected to photon master.</color>");
            photonConnectionWrapper.JoinLobby();
        }

        private void OnDisconnected(string cause)
        {
            DebugHelper.PrintFormatted("<color=red>Disconnected from photon. Reason: {0}</color>.", cause);
        }

        private void OnJoinedLobby()
        {
            DebugHelper.PrintFormatted("<color=green>Joined default lobby.</color>");
        }

        private void OnLeftLobby()
        {
            DebugHelper.PrintFormatted("<color=red>Left default lobby</color>");
        }
        #endregion

        #region Rooms
        public void HostRoom(string roomName, int playerAmount)
        {
            photonRoomWrapper.CreateRoom(roomName, playerAmount);
        }

        public void JoinRoom(string roomName)
        {
            photonRoomWrapper.JoinRoom(roomName);
        }


        public void LeaveRoom()
        {
            photonRoomWrapper.LeaveRoom();
        }


        private void OnRoomJoined(string roomName)
        {
            DebugHelper.PrintFormatted("<color=green>Joined room: {0}.</color>", roomName);
        }

        private void OnJoinRoomFailed(string message)
        {
            DebugHelper.PrintFormatted("<color=red>Joining room failed. Reason: {0}.</color>", message);
        }

        private void OnLeftRoom()
        {
            DebugHelper.PrintFormatted("<color=red>Left room.</color>");
        }
        #endregion
    }
}