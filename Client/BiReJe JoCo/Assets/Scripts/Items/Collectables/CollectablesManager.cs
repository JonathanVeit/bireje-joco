using JoVei.Base;
using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;
using Newtonsoft.Json;
using System.Linq;
using System;
using JoVei.Base.Helper;

namespace BiReJeJoCo.Items
{
    public class CollectableSpawnConfig
    {
        [JsonIgnore]
        public string ItemId => i;
        [JsonIgnore]
        public int SpawnPointIndex => s;
        [JsonIgnore]
        public Vector3? OverridePosition => p;

        /// <summary>
        /// Prefab Id
        /// </summary>
        public string i; 
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
        private Dictionary<string, ICollectable> collectables;
        private Dictionary<int, List<ICollectable>> spawnPointWorkload;

        public Transform Root { get; private set; }
        private Dictionary<string, int> idPool;

        private const string INSTANCE_ID_FORMAT = "{0}_{1}";

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            DontDestroyOnLoad(this);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLobbySceneLoaded);
            DIContainer.RegisterImplementation<CollectablesManager>(this);
            ClearAll();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ClearAll()
        {
            collectables = new Dictionary<string, ICollectable>();
            spawnPointWorkload = new Dictionary<int, List<ICollectable>>();
            Root = null;
            idPool = new Dictionary<string, int>();
        }

        void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<CollectItemPhoMsg>(this, OnItemCollected);
            photonMessageHub.RegisterReceiver<DefinedMatchRulesPhoMsg>(this, OnMatchRulesDefined);
            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnMatchFinished);
            messageHub.UnregisterReceiver(this);
        }
        void DisconnectEvents()
        {
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Access
        public ICollectable[] AllCollectables => collectables.Values.ToArray();
        public ICollectable[] GetAllCollectables(Func<ICollectable, bool> predicate)
        {
            return GetAllCollectablesAs<ICollectable>(predicate);
        }
        public TCollectable[] GetAllCollectablesAs<TCollectable>(Func<ICollectable, bool> predicate)
            where TCollectable : ICollectable
        {
            var result = new List<TCollectable>();
            foreach (ICollectable collectable in collectables.Values)
            {
                if (predicate(collectable))
                    result.Add((TCollectable)collectable);
            }

            return result.ToArray();
        }

        public bool HasCollectableAtIndex(int index)
        {
            if (!spawnPointWorkload.ContainsKey(index))
                return false;

            return spawnPointWorkload[index].Count > 0;
        }
        #endregion

        public ICollectable CreateCollectable(CollectableSpawnConfig config)
        {
            if (Root == null)
                CreateItemRoot();

            var spawnPoint = matchHandler.MatchConfig.mapConfig.GetCollectableSpawnPoint(config.SpawnPointIndex);
            if (config.OverridePosition.HasValue)
                spawnPoint = config.p.Value;

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(config.ItemId);
            var instance = Instantiate(prefab, spawnPoint, Quaternion.identity);
            instance.transform.SetParent(Root);

            var collectable = instance.GetComponent<ICollectable>();
            collectable.InitializeCollectable(GetInstanceId(config.ItemId), config.SpawnPointIndex);
            RegisterCollectableItem(collectable);

            return collectable;
        }

        public void RegisterCollectableItem(ICollectable collectable)
        {
            collectables.Add(collectable.InstanceId, collectable);

            if (!spawnPointWorkload.ContainsKey(collectable.SpawnPointIndex))
                spawnPointWorkload.Add(collectable.SpawnPointIndex, new List<ICollectable>());
            spawnPointWorkload[collectable.SpawnPointIndex].Add(collectable);

            messageHub.ShoutMessage(this, new CollectableItemCreated(collectable.UniqueId, collectable.InstanceId));

            if (globalVariables.GetVar<bool>("debug_mode"))
                DebugHelper.Print(LogType.Log, $"Created collectable {collectable.InstanceId} ({collectable.UniqueId}).");
        }

        public bool HasCollectable(string instanceId)
        {
            return collectables.ContainsKey(instanceId);
        }

        public void CollectItem(ICollectable item)
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

        private void OnMatchRulesDefined(PhotonMessage msg)
        {
            var sceneConfig = matchHandler.MatchConfig.mapConfig;
            for (int i = 0; i < sceneConfig.GetCollectableSpawnPointCount(); i++)
            {
                spawnPointWorkload.Add(i, new List<ICollectable>());
            }
        }

        private void OnItemCollected(PhotonMessage msg)
        {
            var castedMsg = msg as CollectItemPhoMsg;

            if (collectables.ContainsKey(castedMsg.InstanceId))
            {
                var collectable = collectables[castedMsg.InstanceId];
                var itemId = collectable.UniqueId;

                collectable.OnCollect();

                spawnPointWorkload[collectable.SpawnPointIndex].Remove(collectable);
                collectables.Remove(collectable.InstanceId);

                messageHub.ShoutMessage<ItemCollectedByPlayerMsg>(this, castedMsg.playerNumber, itemId);

                if (globalVariables.GetVar<bool>("debug_mode"))
                    DebugHelper.Print(LogType.Log, $"Collected collectable {collectable.InstanceId} ({collectable.UniqueId}).");
            }
            else
            {
                Debug.LogError($"No Collectable with instance id {castedMsg.InstanceId}.");
            }
        }

        private void OnMatchFinished(PhotonMessage msg) 
        {
            ClearAll();
        }
        #endregion

        #region Helper
        private void CreateItemRoot()
        {
            Root = new GameObject("collectables_root").transform;
        }

        public string GetInstanceId(string itemId) 
        {
            if (!idPool.ContainsKey(itemId))
                idPool.Add(itemId, 0);

            idPool[itemId]++;
            string result = string.Format(INSTANCE_ID_FORMAT, itemId, idPool[itemId].ToString());
            while (HasCollectable(result)) 
            {
                idPool[itemId]++;
                result = string.Format(INSTANCE_ID_FORMAT, itemId, idPool[itemId].ToString());
            }

            return result;
        }
        #endregion
    }
}