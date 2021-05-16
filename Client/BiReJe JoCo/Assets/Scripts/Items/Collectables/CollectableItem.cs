using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class CollectableItem : LocalTrigger, ICollectableItem
    {
        [Header("Settings")]
        [SerializeField] string uniqueId;

        public string InstanceId { get; private set; }
        public string UniqueId => uniqueId;

        public void InitializeCollectable(string instanceId)
        {
            InstanceId = instanceId;
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("Collect");
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            collectablesManager.CollectItem(InstanceId);
            foreach (var curFloaty in floaties.Values)
            {
                if (curFloaty)
                    curFloaty.RequestDestroyFloaty();
            }
        }
    }
}