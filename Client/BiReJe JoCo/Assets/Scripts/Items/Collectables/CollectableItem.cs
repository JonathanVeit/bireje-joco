using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class CollectableItem : LocalTrigger, ICollectableItem
    {
        [Header("Collectable Settings")]
        [SerializeField] string uniqueId;

        public string InstanceId { get; private set; }
        public string UniqueId => uniqueId;
        protected bool wasCollected = false;

        public void InitializeCollectable(string instanceId)
        {
            InstanceId = instanceId;
            wasCollected = false;
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("Collect");
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            if (wasCollected) return;

            DisconnectEvents();

            collectablesManager.CollectItem(InstanceId);
            foreach (var curFloaty in floaties.Values)
            {
                if (curFloaty)
                    curFloaty.RequestDestroyFloaty();
            }
            wasCollected = true;
        }

        public virtual void OnCollect()
        {
        }
    }
}