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
            messageHub.RegisterReceiver<ConnectedToPhotonMasterMsg>(this, OnConnectedToMaster);
            messageHub.RegisterReceiver<DisconnectedFromPhotonMsg>(this, OnDisconnected);
            messageHub.RegisterReceiver<JoinedPhotonLobbyMsg>(this, OnJoinedPhotonLobby);
            messageHub.RegisterReceiver<LeftPhotonLobbyMsg>(this, OnLeftPhotonLobby);

            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
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
            DebugHelper.PrintFormatted("<color=green>[Photon Client].</color> Building connecting to photon.");
            photonConnectionWrapper.Connect();
        }

        private void OnConnectedToMaster(ConnectedToPhotonMasterMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Client]</color> Successfully connected to photon master.");
            photonConnectionWrapper.JoinLobby();
        }

        private void OnDisconnected(DisconnectedFromPhotonMsg msg)
        {          
            DebugHelper.PrintFormatted("<color=red>[Photon Client]</color> Disconnected from photon. Reason: {0}.", msg.Param1);
        }

        private void OnJoinedPhotonLobby(JoinedPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Client]</color> Joined default photon lobby.");
        }

        private void OnLeftPhotonLobby(LeftPhotonLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Client]</color> Left default photon lobby.");
        }
        #endregion

        #region Rooms
        public void HostLobby(int playerAmount)
        {
            photonRoomWrapper.CreateRoom(localPlayer.NickName, playerAmount);
        }

        public void JoinLobby(string lobbyName)
        {
            photonRoomWrapper.JoinRoom(lobbyName);
        }

        public void LeaveLobby()
        {
            photonRoomWrapper.LeaveRoom();
        }


        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=green>[Photon Client]</color> Joined game lobby: {0}.", msg.Param1);
        }

        private void OnJoinLobbyFailed(JoinLobbyFailedMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Client]</color> Joining game lobby failed. Reason: {0}.", msg.Param1);
        }

        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            DebugHelper.PrintFormatted("<color=red>[Photon Client]</color> Left game lobby.");
        }
        #endregion
    }
}