using BiReJeJoCo.Character;
using Newtonsoft.Json;
using System;
using PhotonPlayer = Photon.Realtime.Player;

namespace BiReJeJoCo.Backend
{
    public class Player : SystemAccessor
    {
        #region Keys
        protected const string PLAYER_STATE_KEY     = "State";
        protected const string PLAYER_ROLE_KEY      = "Role";
        protected const string PREFERED_ROLE_KEY    = "PreferedRole";
        protected const string READY_TO_START_KEY   = "ReadyToStart";
        #endregion

        public bool IsLocalPlayer => photonPlayer.IsLocal;
        public bool IsHost => photonPlayer.IsMasterClient;
        public PlayerState State => LoadPlayerState();

        public PlayerRole Role => LoadPlayerRole();
        public PlayerRole PreferedRole => LoadPreferedPlayerRole();
        public bool ReadyToStart => LoadReadyToStart();

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
            var rawState = photonPlayer.CustomProperties[PLAYER_STATE_KEY].ToString();
            return (PlayerState) Enum.Parse(typeof(PlayerState), rawState);
        }

        private PlayerRole LoadPlayerRole()
        {
            var rawState = photonPlayer.CustomProperties[PLAYER_ROLE_KEY].ToString();
            return (PlayerRole)Enum.Parse(typeof(PlayerRole), rawState);
        }

        private PlayerRole LoadPreferedPlayerRole() 
        {
            var rawState = photonPlayer.CustomProperties[PREFERED_ROLE_KEY].ToString();
            return (PlayerRole)Enum.Parse(typeof(PlayerRole), rawState);
        }

        private bool LoadReadyToStart()
        {
            var rawValue = photonPlayer.CustomProperties[READY_TO_START_KEY].ToString();
            return bool.Parse(rawValue);
        }
        
        #endregion
    }
}