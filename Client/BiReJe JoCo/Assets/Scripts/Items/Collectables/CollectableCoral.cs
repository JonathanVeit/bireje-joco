using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo
{
    public class CollectableCoral : SystemBehaviour, ICollectableItem
    {
        [Header("Collectable Settings")]
        [SerializeField] string uniqueId;

        private bool isCollected;

        public string InstanceId { get; private set; }
        public string UniqueId => uniqueId;

        public void InitializeCollectable(string instanceId)
        {
            InstanceId = instanceId;
        }

        public void Destroy() 
        {
            if (isCollected) return;

            collectablesManager.CollectItem(this);
            isCollected = true;
        }

        public void OnCollect() 
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("coral_destroy_sfx");
            poolingManager.PoolInstance(prefab, transform.position, transform.rotation);
        }
    }
}