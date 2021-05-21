using BiReJeJoCo.Character;
using Newtonsoft.Json;
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
        public PlayerRole PreferedRole => LoadPreferedPlayerRole();
        public string NickName => photonPlayer.NickName;
        public string Id => photonPlayer.UserId;
        public int NumberInRoom => photonPlayer.ActorNumber;
        protected PhotonPlayer photonPlayer;

        [JsonIgnore]
        public CharacterSetup PlayerCharacter { get; protected set; }

        public Player(PhotonPlayer player) 
        {
            photonPlayer = player;
        }

        public void AssignCharacter(CharacterSetup character)
        {
            PlayerCharacter = character;
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

        private PlayerRole LoadPreferedPlayerRole() 
        {
            var rawState = photonPlayer.CustomProperties["PreferedRole"].ToString();
            return (PlayerRole)Enum.Parse(typeof(PlayerRole), rawState);
        }
        #endregion
    }
}