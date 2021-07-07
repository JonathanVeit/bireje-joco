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
                { "State", PlayerState.Free },
                { "Role", PlayerRole.None },
                { "PreferedRole", (PlayerRole) PlayerPrefs.GetInt("PreferedRole") },
                { "ReadyToStart", false }
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
            UpdateProperty("Role", role);
        }
        public void SetPreferedRole(PlayerRole role)
        {
            UpdateProperty("PreferedRole", role);
        }

        public void SetReadyToStart(bool ready)
        {
            UpdateProperty("ReadyToStart", ready);
        }

        #region Events
        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            UpdateProperties("State", PlayerState.Lobby, "Role", PlayerRole.Spectator, "ReadyToStart", false);
        }

        private void OnJoinedPhotonLobby(JoinedPhotonLobbyMsg msg) 
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