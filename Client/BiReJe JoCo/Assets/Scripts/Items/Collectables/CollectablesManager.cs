using JoVei.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Items
{
    public class CollectableSpawnConfig
    {
        public string i; // prefab id  
        public string i2; // instance id
        public int s; // index of spawnpoint
    }

    public class CollectablesManager : SystemBehaviour, IInitializable
    {
        private Dictionary<string, ICollectableItem> items;
        private Transform root;

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            items = new Dictionary<string, ICollectableItem>();
            ConnectEvents();

            DIContainer.RegisterImplementation<CollectablesManager>(this);
            yield return null;
        }
        public void CleanUp() { DisconnectEvents(); }

        void ConnectEvents()
        {
            messageHub.RegisterReceiver<LoadedGameSceneMsg>(this, OnLoadedGameScene);
        }

        void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public void CreateCollectable(CollectableSpawnConfig config)
        {
            if (root == null)
                CreateItemRoot();
            var scene = matchHandler.MatchConfig.matchScene;
            var spawnPoint = MapConfigMapping.GetMapping().GetElementForKey(scene).GetCollectableSpawnPoint(config.s);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(config.i);
            var instance = Instantiate(prefab, spawnPoint, Quaternion.identity);
            instance.transform.SetParent(root);

            var item = instance.GetComponent<ICollectableItem>();
            item.InitializeCollectable(config.i2);
            items.Add(config.i2, item);
        }

        public void CollectItem(string instanceId)
        {
            photonMessageHub.ShoutMessage(new CollectItemPhoMsg(instanceId, localPlayer.NumberInRoom), PhotonMessageTarget.All);
        }

        #region Events
        private void OnLoadedGameScene(LoadedGameSceneMsg msg)
        {
            items = new Dictionary<string, ICollectableItem>();
            photonMessageHub.RegisterReceiver<CollectItemPhoMsg>(this, OnItemCollected);
        }

        private void OnItemCollected(PhotonMessage msg)
        {
            var castedMsg = msg as CollectItemPhoMsg;
            var itemId = items[castedMsg.i].UniqueId;
            Destroy((items[castedMsg.i] as Component).gameObject);
            items.Remove(castedMsg.i);

            if (castedMsg.i2 == localPlayer.NumberInRoom)
            {
                messageHub.ShoutMessage<ItemCollectedByPlayerMsg>(this, itemId);
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