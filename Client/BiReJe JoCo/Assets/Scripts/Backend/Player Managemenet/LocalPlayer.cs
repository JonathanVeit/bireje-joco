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
                { "State", PlayerState.Free }
            };

            rootPlayer.SetCustomProperties(properties);
        }
        
        private void ConnectToEvents()
        {
            messageHub.RegisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftRoom);
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


        #region Events
        private void OnJoinedLobby(OnJoinLobbyFailedMsg msg)
        {
            UpdateState(PlayerState.Lobby);
        }

        private void OnLeftRoom(OnLeftLobbyMsg msg) 
        {
            UpdateState(PlayerState.Free);
        }

        private void OnGameSceneLoaded(OnLoadedGameSceneMsg msg)
        {
            SpawnPlayerCharacter();
            UpdateState(PlayerState.Ready);
        }
        #endregion

        #region Helper
        private void UpdateState(PlayerState state)
        {
            var properties = rootPlayer.CustomProperties;

            properties["State"] = state;
            rootPlayer.SetCustomProperties(properties);
        }
        #endregion
    }
}