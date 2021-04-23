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
                { "Role", PlayerRole.None}
            };

            PhotonPlayer.SetCustomProperties(properties);
        }
        
        private void ConnectToEvents()
        {
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnJoinedPhotonLobbyMsg>(this, OnJoinedPhotonLobby);
            messageHub.RegisterReceiver<OnLoadedGameSceneMsg>(this, OnGameSceneLoaded);
        }
        #endregion

        public GameObject PlayerCharacter { get; private set; }

        private void SpawnPlayerCharacter() 
        {
            string prefabId = PlayerPrefabMapping.GetMapping().GetElementForKey("third_person_pc");
            var randomPos = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
            PlayerCharacter = photonRoomWrapper.Instantiate(prefabId, randomPos, Quaternion.identity);
        }


        public void SetNickName(string name)
        {
            PhotonPlayer.NickName = name;
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

        private void OnGameSceneLoaded(OnLoadedGameSceneMsg msg)
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

            var properties = PhotonPlayer.CustomProperties;
            for (int i = 0; i < keyValuePairs.Length; i += 2)
            {
                properties[keyValuePairs[i]] = keyValuePairs[i+1];
            }

            PhotonPlayer.SetCustomProperties(properties);
        }
        #endregion
    }
}