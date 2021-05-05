using PhotonPlayer = Photon.Realtime.Player;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using Newtonsoft.Json;
using UnityEngine;

namespace BiReJeJoCo.Backend
{
    public class LocalPlayer : Player
    {
        #region Initalization
        public LocalPlayer(PhotonPlayer player) : base(player)
        {
            InitializeProperties();
            ConnectToEvents();
        }
        
        private void InitializeProperties()
        {
            var properties = new PhotonHashTable()
            {
                { "State", PlayerState.Free },
                { "Role", PlayerRole.None}
            };

            photonPlayer.SetCustomProperties(properties);
        }
        
        private void ConnectToEvents()
        {
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnJoinedPhotonLobbyMsg>(this, OnJoinedPhotonLobby);
            messageHub.RegisterReceiver<LoadedGameSceneMsg>(this, OnGameSceneLoaded);
        }
        #endregion

        [JsonIgnore] 
        public PlayerControlled PlayerCharacter { get; private set; }

        private void SpawnPlayerCharacter() 
        {
            string prefabId = PlayerPrefabMapping.GetMapping().GetElementForKey("third_person_pc");

            var scene = matchHandler.MatchConfig.matchScene;
            var posIndex = matchHandler.MatchConfig.spawnPos[NumberInRoom];
            var randomPos = Vector3.zero;
            
            if (Role == PlayerRole.Hunted)
                randomPos = MapConfigMapping.GetMapping().GetElementForKey(scene).GetHuntedSpawnPoint (posIndex);
            else if (Role == PlayerRole.Hunter)
                randomPos = MapConfigMapping.GetMapping().GetElementForKey(scene).GetHunterSpawnPoint(posIndex);

            var go = photonRoomWrapper.Instantiate(prefabId, randomPos, Quaternion.identity);
            PlayerCharacter = go.GetComponent<PlayerControlled>();

            messageHub.ShoutMessage<PlayerCharacterSpawnedMsg>(this);
        }

        public void SetNickName(string name)
        {
            photonPlayer.NickName = name;
        }

        public void SetRole(PlayerRole role)
        {
            UpdateProperty("Role", role);
        }

        #region Events
        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            UpdateProperties("State", PlayerState.Lobby, "Role", PlayerRole.Spectator);
        }

        private void OnJoinedPhotonLobby(OnJoinedPhotonLobbyMsg msg) 
        {
            UpdateProperties("State", PlayerState.Free, "Role", PlayerRole.None);
        }

        private void OnGameSceneLoaded(LoadedGameSceneMsg msg)
        {
            SpawnPlayerCharacter();
            UpdateProperty("State", PlayerState.Ready);
        }
        #endregion

        #region Helper
        private void UpdateProperty(string key, object value)
        {
            UpdateProperties(key, value);
        }

        private void UpdateProperties(params object[] keyValuePairs)
        {
            if (keyValuePairs.Length % 2 != 0)
                throw new System.ArgumentException($"Cannot update properties. Missing value for key {keyValuePairs[keyValuePairs.Length-1]}");

            var properties = photonPlayer.CustomProperties;
            for (int i = 0; i < keyValuePairs.Length; i += 2)
            {
                properties[keyValuePairs[i]] = keyValuePairs[i+1];
            }

            photonPlayer.SetCustomProperties(properties);
        }
        #endregion
    }
}