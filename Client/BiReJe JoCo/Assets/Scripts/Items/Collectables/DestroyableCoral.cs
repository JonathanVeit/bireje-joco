using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo
{
    public class DestroyableCoral : SystemBehaviour, ICollectable
    {
        [Header("Collectable Settings")]
        [SerializeField] string uniqueId;

        private bool isCollected;

        public string InstanceId { get; private set; }
        public int SpawnPointIndex { get; private set; }
        public float SizeDelta { get; private set; }
        public string UniqueId => uniqueId;

        public void InitializeCollectable(string instanceId, int spawnPointIndex)
        {
            InstanceId = instanceId;
            SpawnPointIndex = spawnPointIndex;
        }

        public void SetSizeDelta(float value) 
        {
            SizeDelta = value;
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

            soundEffectManager.Play("coral_destroy", transform.position);
            Destroy(transform.parent.gameObject);
        }
    }
}