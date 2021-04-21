using System;
using PhotonPlayer = Photon.Realtime.Player;

namespace BiReJeJoCo.Backend
{
    public class Player : SystemAccessor
    { 
        public bool IsLocalPlayer => rootPlayer.IsLocal;
        public bool IsHost => rootPlayer.IsMasterClient;

        public PlayerState State => CalculcatePlayerState();

        public string NickName => rootPlayer.NickName;
        public string Id => rootPlayer.UserId;
        public int NumberInRoom => rootPlayer.ActorNumber;

        protected PhotonPlayer rootPlayer;


        public Player(PhotonPlayer player) 
        {
            rootPlayer = player;
        }

        private PlayerState CalculcatePlayerState() 
        {
            var rawState = rootPlayer.CustomProperties["State"].ToString();
            return (PlayerState) Enum.Parse(typeof(PlayerState), rawState);
        }
    }
}