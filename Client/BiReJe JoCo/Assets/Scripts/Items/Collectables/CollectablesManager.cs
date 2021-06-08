using JoVei.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;
using Newtonsoft.Json;

namespace BiReJeJoCo.Items
{
    public class CollectableSpawnConfig
    {
        [JsonIgnore]
        public string PrefabId => i;
        [JsonIgnore]
        public string InstanceId => i2;
        [JsonIgnore]
        public int SpawnPointIndex => s;

        /// <summary>
        /// Prefab Id
        /// </summary>
        public string i; 
        /// <summary>
        /// Instance Id
        /// </summary>
        public string i2;
        /// <summary>
        /// Spawnpoint index
        /// </summary>
        public int s;
    }

    public class CollectablesManager : SystemBehaviour
    {
        private Dictionary<string, ICollectableItem> items;
        private Transform root;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            Setup();
            DontDestroyOnLoad(this);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            DIContainer.RegisterImplementation<CollectablesManager>(this);
        }
        protected override void OnBeforeDestroy() 
        { 
            DisconnectEvents(); 
        }

        private void Setup()
        {
            items = new Dictionary<string, ICollectableItem>();
            root = null;
        }

        void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<CollectItemPhoMsg>(this, OnItemCollected);
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnCloseMatch);
            messageHub.UnregisterReceiver(this);
        }
        void DisconnectEvents()
        {
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        public void CreateCollectable(CollectableSpawnConfig config)
        {
            if (root == null)
                CreateItemRoot();
            var scene = matchHandler.MatchConfig.matchScene;
            var spawnPoint = MapConfigMapping.GetMapping().GetElementForKey(scene).GetCollectableSpawnPoint(config.SpawnPointIndex);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(config.PrefabId);
            var instance = Instantiate(prefab, spawnPoint, Quaternion.identity);
            instance.transform.SetParent(root);

            var item = instance.GetComponent<ICollectableItem>();
            item.InitializeCollectable(config.InstanceId);
            items.Add(config.InstanceId, item);
        }

        public void CollectItem(string instanceId)
        {
            photonMessageHub.ShoutMessage(new CollectItemPhoMsg(instanceId, localPlayer.NumberInRoom), PhotonMessageTarget.AllViaServer);
        }

        #region Events
        private void OnLobbySceneLoaded(LoadedLobbySceneMsg msg)
        {
            ConnectEvents();
        }
        private void OnCloseMatch(PhotonMessage msg)
        {
            Setup();
        }

        private void OnItemCollected(PhotonMessage msg)
        {
            var castedMsg = msg as CollectItemPhoMsg;

            if (items.ContainsKey(castedMsg.InstanceId))
            {
                var itemId = items[castedMsg.InstanceId].UniqueId;
                Destroy((items[castedMsg.InstanceId] as Component).gameObject);
                items.Remove(castedMsg.InstanceId);

                if (castedMsg.playerNumber == localPlayer.NumberInRoom)
                {
                    messageHub.ShoutMessage<ItemCollectedByPlayerMsg>(this, itemId);
                }
            } 
            else
            {
                Debug.LogError($"No Collectable with instance id {castedMsg.InstanceId}.");
            }
        }
        #endregion

        #region Helper
        private void CreateItemRoot()
        {
            root = new GameObject("collectables_root").transform;
        }
        #endregion
    }
}