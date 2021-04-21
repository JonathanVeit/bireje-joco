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
            messageHub.RegisterReceiver<OnConnectedToPhotonMasterMsg>(this, OnConnectedToMaster);
            messageHub.RegisterReceiver<OnDisconnectedFromPhotonMsg>(this, OnDisconnected);
            messageHub.RegisterReceiver<OnJoinedPhotonLobbyMsg>(this, OnJoinedPhotonLobby);
            messageHub.RegisterReceiver<OnLeftPhotonLobbyMsg>(this, OnLeftPhotonLobby);

            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnRoomJoined);
            messageHub.RegisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinRoomFailed);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftRoom);
        }

        private void DisconnectEvents()
        {
        }
        #endregion

        #region Connection
        public void BuildConnection()
        {
            DebugHelper.PrintFormatted("Building connecting to photon.");
            photonConnectionWrapper.Connect();
        }

        private void OnConnectedToMaster(OnConnectedToPhotonMasterMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>Successfully connected to photon master.</color>");
            photonConnectionWrapper.JoinLobby();
        }

        private void OnDisconnected(OnDisconnectedFromPhotonMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>Disconnected from photon. Reason: {0}</color>.", msg.Param1);
        }

        private void OnJoinedPhotonLobby(OnJoinedPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>Joined default photon lobby.</color>");
        }

        private void OnLeftPhotonLobby(OnLeftPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>Left default photon lobby</color>");
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


        private void OnRoomJoined(OnJoinedLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>Joined room: {0}.</color>", msg.Param1);
        }

        private void OnJoinRoomFailed(OnJoinLobbyFailedMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>Joining room failed. Reason: {0}.</color>", msg.Param1);
        }

        private void OnLeftRoom(OnLeftLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>Left room.</color>");
        }
        #endregion
    }
}