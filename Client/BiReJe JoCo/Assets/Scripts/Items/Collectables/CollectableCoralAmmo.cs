using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using System;
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

        private HuntedBehaviour huntedBehaviour 
            => playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted)[0].PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>();

        #region Initialization
        public void InitializeCollectable(string instanceId, int spawnPoinIndex)
        {
            InstanceId = instanceId;
            SpawnPointIndex = spawnPoinIndex;
            wasCollected = false;
        }

        protected override void ConnectEvents()
        {
            base.ConnectEvents();
            huntedBehaviour.CoralMechanic.onSpawnedCorals += OnCoralsSpawned;
        }
        protected override void DisconnectEvents()
        {
            base.DisconnectEvents();
            huntedBehaviour.CoralMechanic.onSpawnedCorals -= OnCoralsSpawned;
        }
        #endregion

        #region Events
        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            blockInteraction = huntedBehaviour.CoralMechanic.AmmoIsFull;
            floaty.SetDescription(blockInteraction ? "Already full" : "Collect");
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
            soundEffectManager.Play("hunted_collect_item", transform.position);
            DisconnectEvents();
            Destroy(this.gameObject);
        }

        private void OnCoralsSpawned()
        {
            foreach (var trigger in triggerPoints)
            {
                DestroyTriggerFloaty(trigger);
            }

            ResetActiveInstance();
        }
        #endregion
    }
}