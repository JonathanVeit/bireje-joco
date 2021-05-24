using JoVei.Base;
using JoVei.Base.MessageSystem;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

namespace BiReJeJoCo.Backend
{
    public class PhotonConnectionWrapper : MonoBehaviourPunCallbacks, IInitializable
    {
        public bool IsConnected { get; private set; }
        public bool IsConnectedToMaster { get; private set; }
        public bool IsInLobby { get; private set; }

        private IMessageHub messageHub => DIContainer.GetImplementationFor<IMessageHub>();

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            SetupPhoton();
            DIContainer.RegisterImplementation<PhotonConnectionWrapper>(this);
            yield return null;
        }

        private void SetupPhoton() 
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.PhotonServerSettings.DevRegion = "eu";
        }

        public void CleanUp() { }
        #endregion

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnected()
        {
            messageHub.ShoutMessage(this, new ConnectedToPhotonMsg());
            IsConnected = true;
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            messageHub.ShoutMessage(this, new DisconnectedFromPhotonMsg(cause.ToString()));
            IsConnectedToMaster = false;
            IsConnected = false;
        }
        public override void OnConnectedToMaster()
        {
            messageHub.ShoutMessage(this, new ConnectedToPhotonMasterMsg());
            IsConnectedToMaster = true;
        }


        public void JoinLobby() 
        {
            PhotonNetwork.JoinLobby();
        }
        public override void OnJoinedLobby()
        {
            messageHub.ShoutMessage(this, new JoinedPhotonLobbyMsg());
            IsInLobby = true;
        }
        public override void OnLeftLobby()
        {
            messageHub.ShoutMessage(this, new LeftPhotonLobbyMsg());
            IsInLobby = false;
        }
    }
}