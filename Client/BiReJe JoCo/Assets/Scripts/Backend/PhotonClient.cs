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

            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<JoinLobbyFailedMsg>(this, OnJoinLobbyFailed);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
        }

        private void DisconnectEvents()
        {
        }
        #endregion

        #region Connection
        public void BuildConnection()
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Clien].</color> Building connecting to photon.");
            photonConnectionWrapper.Connect();
        }

        private void OnConnectedToMaster(OnConnectedToPhotonMasterMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Clien]</color> Successfully connected to photon master.");
            photonConnectionWrapper.JoinLobby();
        }

        private void OnDisconnected(OnDisconnectedFromPhotonMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Clien]</color> Disconnected from photon. Reason: {0}.", msg.Param1);
        }

        private void OnJoinedPhotonLobby(OnJoinedPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Clien]</color> Joined default photon lobby.");
        }

        private void OnLeftPhotonLobby(OnLeftPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Clien]</color> Left default photon lobby.");
        }
        #endregion

        #region Rooms
        public void HostLobby(string lobbyName, int playerAmount)
        {
            photonRoomWrapper.CreateRoom(lobbyName, playerAmount);
        }

        public void JoinLobby(string lobbyName)
        {
            photonRoomWrapper.JoinRoom(lobbyName);
        }

        public void LeaveLobby()
        {
            photonRoomWrapper.LeaveRoom();
        }


        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Clien]</color> Joined game lobby: {0}.", msg.Param1);
        }

        private void OnJoinLobbyFailed(JoinLobbyFailedMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Clien]</color> Joining game lobby failed. Reason: {0}.", msg.Param1);
        }

        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Clien]</color> Left game lobby.");
        }
        #endregion
    }
}