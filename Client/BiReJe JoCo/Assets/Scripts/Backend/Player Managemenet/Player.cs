using System;
using PhotonPlayer = Photon.Realtime.Player;

namespace BiReJeJoCo.Backend
{
    public class Player : SystemAccessor
    { 
        public bool IsLocalPlayer => PhotonPlayer.IsLocal;
        public bool IsHost => PhotonPlayer.IsMasterClient;
        public PlayerState State => LoadPlayerState();

        public PlayerRole Role => LoadPlayerRole();
        public string NickName => PhotonPlayer.NickName;
        public string Id => PhotonPlayer.UserId;
        public int NumberInRoom => PhotonPlayer.ActorNumber;

        public PhotonPlayer PhotonPlayer { get; private set; }

        public Player(PhotonPlayer player) 
        {
            PhotonPlayer = player;
        }

        #region Helper
        private PlayerState LoadPlayerState() 
        {
            var rawState = PhotonPlayer.CustomProperties["State"].ToString();
            return (PlayerState) Enum.Parse(typeof(PlayerState), rawState);
        }

        private PlayerRole LoadPlayerRole()
        {
            var rawState = PhotonPlayer.CustomProperties["Role"].ToString();
            return (PlayerRole)Enum.Parse(typeof(PlayerRole), rawState);
        }
        #endregion
    }
}