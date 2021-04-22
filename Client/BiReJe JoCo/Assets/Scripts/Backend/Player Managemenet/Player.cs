using System;
using PhotonPlayer = Photon.Realtime.Player;

namespace BiReJeJoCo.Backend
{
    public class Player : SystemAccessor
    { 
        public bool IsLocalPlayer => photonPlayer.IsLocal;
        public bool IsHost => photonPlayer.IsMasterClient;
        public PlayerState State => LoadPlayerState();


        public PlayerRole Role => LoadPlayerRole();
        public string NickName => photonPlayer.NickName;
        public string Id => photonPlayer.UserId;
        public int NumberInRoom => photonPlayer.ActorNumber;


        protected PhotonPlayer photonPlayer;

        public Player(PhotonPlayer player) 
        {
            photonPlayer = player;
        }

        #region Helper
        private PlayerState LoadPlayerState() 
        {
            var rawState = photonPlayer.CustomProperties["State"].ToString();
            return (PlayerState) Enum.Parse(typeof(PlayerState), rawState);
        }

        private PlayerRole LoadPlayerRole()
        {
            var rawState = photonPlayer.CustomProperties["Role"].ToString();
            return (PlayerRole)Enum.Parse(typeof(PlayerRole), rawState);
        }
        #endregion
    }
}