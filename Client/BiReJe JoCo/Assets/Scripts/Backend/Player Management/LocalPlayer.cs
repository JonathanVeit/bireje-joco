using PhotonPlayer = Photon.Realtime.Player;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
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
                { PLAYER_STATE_KEY, PlayerState.Free },
                { PLAYER_ROLE_KEY, PlayerRole.None },
                { PREFERED_ROLE_KEY, (PlayerRole) PlayerPrefs.GetInt(PREFERED_ROLE_KEY) },
                { READY_TO_START_KEY, false }
            };

            photonPlayer.SetCustomProperties(properties);
        }
        
        private void ConnectToEvents()
        {
            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<JoinedPhotonLobbyMsg>(this, OnJoinedPhotonLobby);
            messageHub.RegisterReceiver<LoadedGameSceneMsg>(this, OnGameSceneLoaded);
        }
        #endregion

        private void SpawnPlayerCharacter() 
        {
            string prefabId = MatchPrefabMapping.GetMapping().GetElementForKey("player_character").name;

            var posIndex = matchHandler.MatchConfig.spawnPos[NumberInRoom];
            var randomPos = Vector3.zero;

            if (Role == PlayerRole.Hunted)
                randomPos = matchHandler.MatchConfig.mapConfig.GetHuntedSpawnPoint (posIndex);
            else if (Role == PlayerRole.Hunter)
                randomPos = matchHandler.MatchConfig.mapConfig.GetHunterSpawnPoint(posIndex);

            var go = photonRoomWrapper.Instantiate(prefabId, randomPos, Quaternion.identity);

            messageHub.ShoutMessage<PlayerCharacterSpawnedMsg>(this);
        }
        public void DestroyPlayerCharacter()
        {
            if (!PlayerCharacter)
                return;

            photonRoomWrapper.Destroy(PlayerCharacter.gameObject);
        }

        public void SetNickName(string name)
        {
            photonPlayer.NickName = name;
        }
        public void SetRole(PlayerRole role)
        {
            UpdateProperty(PLAYER_ROLE_KEY, role);
        }
        public void SetPreferedRole(PlayerRole role)
        {
            UpdateProperty(PREFERED_ROLE_KEY, role);
            PlayerPrefs.SetInt(PREFERED_ROLE_KEY, (int) role);
        }

        public void SetReadyToStart(bool ready)
        {
            UpdateProperty(READY_TO_START_KEY, ready);
        }

        #region Events
        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            UpdateProperties(PLAYER_STATE_KEY, PlayerState.Lobby, PLAYER_ROLE_KEY, PlayerRole.Spectator, READY_TO_START_KEY, false);
        }

        private void OnJoinedPhotonLobby(JoinedPhotonLobbyMsg msg) 
        {
            UpdateProperties(PLAYER_STATE_KEY, PlayerState.Free, PLAYER_ROLE_KEY, PlayerRole.None);
        }

        private void OnGameSceneLoaded(LoadedGameSceneMsg msg)
        {
            SpawnPlayerCharacter();
            UpdateProperty(PLAYER_STATE_KEY, PlayerState.Ready);
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