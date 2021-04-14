using System.Collections;
using UnityEngine;
using JoVei.Base;
using JoVei.Base.Helper;

namespace BiReJeJoCo
{
    public class PhotonClient : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonClient>(this);
            ConnectEvents();

            BuildConnection();
            yield return new WaitUntil(() => photonWrapper.IsInLobby);
        }

        public void CleanUp()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            photonWrapper.OnConnectedToPhotonMaster += OnConnectedToMaster;
            photonWrapper.OnDisconnectedFromPhoton += OnDisconnected;
            photonWrapper.OnJoinedPhotonLobby += OnJoinedLobby;
            photonWrapper.OnLeftPhotonLobby += OnLeftLobby;
            photonWrapper.OnJoinedPhotonRoom += OnRoomJoined;
            photonWrapper.OnJoinPhotonRoomFailed += OnJoinRoomFailed;
        }

        private void DisconnectEvents()
        {
            photonWrapper.OnConnectedToPhotonMaster -= OnConnectedToMaster;
            photonWrapper.OnDisconnectedFromPhoton -= OnDisconnected;
            photonWrapper.OnJoinedPhotonLobby -= OnJoinedLobby;
            photonWrapper.OnLeftPhotonLobby -= OnLeftLobby;
            photonWrapper.OnJoinedPhotonRoom -= OnRoomJoined;
            photonWrapper.OnJoinPhotonRoomFailed -= OnJoinRoomFailed;
        }
        #endregion

        #region Connection
        public void BuildConnection()
        {
            DebugHelper.PrintFormatted("Connecting to photon.");
            photonWrapper.ConnectToPhoton();
        }

        private void OnConnectedToMaster()
        {
            DebugHelper.PrintFormatted("<color=green>Successfully connected to photon master.</color>");
            photonWrapper.JoinLobby();
        }

        private void OnDisconnected(Photon.Realtime.DisconnectCause cause)
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

        #region Room Management
        public void HostRoom(string roomName, int playerAmount)
        {
            photonWrapper.CreateRoom(roomName, playerAmount);
        }

        public void JoinRoom(string roomName)
        {
            photonWrapper.JoinRoom(roomName);
        }

        private void OnRoomJoined(string roomName)
        {
            DebugHelper.PrintFormatted("<color=green>Joined room: {0}.</color>", roomName);
        }

        private void OnJoinRoomFailed(string message)
        {
            DebugHelper.PrintFormatted("<color=red>Joining room failed. Reason: {0}.</color>", message);
        }
        #endregion
    }
}