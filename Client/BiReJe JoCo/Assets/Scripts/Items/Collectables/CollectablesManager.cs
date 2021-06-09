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
        public string ItemId => i;
        [JsonIgnore]
        public string InstanceId => i2;
        [JsonIgnore]
        public int SpawnPointIndex => s;
        [JsonIgnore]
        public Vector3? OverridePosition => p;

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
        /// <summary>
        /// overridable spawnposition
        /// </summary>
        public Vector3? p;
    }

    public class CollectablesManager : SystemBehaviour
    {
        private Dictionary<string, ICollectableItem> items;
        public Transform Root { get; private set; }

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
            Root = null;
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

        public ICollectableItem CreateCollectable(CollectableSpawnConfig config)
        {
            if (Root == null)
                CreateItemRoot();
            var scene = matchHandler.MatchConfig.matchScene;
            var spawnPoint = MapConfigMapping.GetMapping().GetElementForKey(scene).GetCollectableSpawnPoint(config.SpawnPointIndex);
            if (config.OverridePosition.HasValue)
                spawnPoint = config.p.Value;

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(config.ItemId);
            var instance = Instantiate(prefab, spawnPoint, Quaternion.identity);
            instance.transform.SetParent(Root);

            var item = instance.GetComponent<ICollectableItem>();
            item.InitializeCollectable(config.InstanceId);
            RegisterCollectableItem(item);
            return item;
        }

        public void RegisterCollectableItem(ICollectableItem item)
        {
            items.Add(item.InstanceId, item);
            messageHub.ShoutMessage(this, new CollectableItemCreated(item.UniqueId, item.InstanceId));
        }

        public void CollectItem(ICollectableItem item)
        {
            CollectItem(item.InstanceId);
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
                items[castedMsg.InstanceId].OnCollect();
                Destroy((items[castedMsg.InstanceId] as Component).gameObject);
                items.Remove(castedMsg.InstanceId);

                messageHub.ShoutMessage<ItemCollectedByPlayerMsg>(this, castedMsg.playerNumber, itemId);
            }
            else
            {
                Debug.Log($"No Collectable with instance id {castedMsg.InstanceId}.");
            }
        }
        #endregion

        #region Helper
        private void CreateItemRoot()
        {
            Root = new GameObject("collectables_root").transform;
        }
        #endregion
    }
}