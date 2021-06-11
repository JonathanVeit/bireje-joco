using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class CollectableCoralAmmo : LocalTrigger, ICollectable
    {
        [Header("Collectable Settings")]
        [SerializeField] string uniqueId;

        public string InstanceId { get; private set; }
        public int SpawnPointIndex { get; private set; }
        public string UniqueId => uniqueId;
        protected bool wasCollected = false;

        public void InitializeCollectable(string instanceId, int spawnPoinIndex)
        {
            InstanceId = instanceId;
            SpawnPointIndex = spawnPoinIndex;
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

        protected override bool PlayerIsInArea(TriggerSetup trigger)
        {
            var huntedCharacter = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted)[0].PlayerCharacter;
            if (huntedCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().CoralMechanic.AmmoIsFull)
                return false;

            return base.PlayerIsInArea(trigger);
        }

        public virtual void OnCollect()
        {
        }
    }
}