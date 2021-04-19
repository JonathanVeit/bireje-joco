using JoVei.Base;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

namespace BiReJeJoCo.Backend
{
    public class PhotonConnectionWrapper : MonoBehaviourPunCallbacks, IInitializable
    {
        public event Action onConnected;
        public event Action<string> onDisconnected;
        public event Action onConnectedToMaster;

        public event Action onJoinedLobby;
        public event Action onLeftLobby;

        public string NickName { get => PhotonNetwork.NickName; set { PhotonNetwork.NickName = value; } }

        public bool IsConnected { get; private set; }
        public bool IsConnectedToMaster { get; private set; }
        public bool IsInLobby { get; private set; }

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PhotonConnectionWrapper>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        public void Connect ()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnected()
        {
            onConnected?.Invoke();
            IsConnected = true;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            onDisconnected?.Invoke(cause.ToString());
            IsConnectedToMaster = false;
            IsConnected = false;
        }
    
        public override void OnConnectedToMaster()
        {
            onConnectedToMaster?.Invoke();
            IsConnectedToMaster = true;
        }


        public void JoinLobby() 
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            onJoinedLobby?.Invoke();
            IsInLobby = true;
        }

        public override void OnLeftLobby()
        {
            onLeftLobby?.Invoke();
            IsInLobby = false;
        }
    }
}