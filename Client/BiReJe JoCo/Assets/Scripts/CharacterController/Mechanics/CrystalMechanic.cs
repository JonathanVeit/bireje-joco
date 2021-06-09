using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CrystalMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Transform spawnPoint;
        [SerializeField] int maxCrystals;
        [SerializeField] int crystalsPerCollectable;
        [SerializeField] string collectableId = "collectable_crystal";

        [Header("Runtime")]
        [SerializeField] int crystalAmmo = 0;
       
        public int TotalCrystals { get; private set; } = 0;

        private SyncVar<object[]> onSpawnCrystals = new SyncVar<object[]>(8, true);

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
            gameUI.UpdateCrystalAmmoBar(crystalAmmo / (float)maxCrystals);
        }
        protected override void OnInitializeRemote()
        {
            ConnectEvents();
            onSpawnCrystals.OnValueReceived += SpawnCrystalsInternal;
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<ItemCollectedByPlayerMsg>(this, OnItemCollected);
        }
        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(onSpawnCrystals);
        }
        #endregion

        public void SpawnCrystals() 
        {
            if (crystalAmmo == 0)
                return;

            var args = new object[3] 
            {
                spawnPoint.position, 
                Random.Range(0, int.MaxValue),
                System.Guid.NewGuid().ToString(),
            };

            onSpawnCrystals.SetValue(args);
            SpawnCrystalsInternal(onSpawnCrystals.GetValue());

            crystalAmmo--;
            gameUI.UpdateCrystalAmmoBar(crystalAmmo / (float)maxCrystals);
        }

        private void SpawnCrystalsInternal(object[] args) 
        {
            var pos = (Vector3)args[0];
            var seed = (int)args[1];
            var instanceId = (string)args[2];

            var config = new CollectableSpawnConfig()
            {
                i = collectableId,
                i2 = instanceId,
                p = pos,
            };

            var crystalSpawner = (CollectableCrystal) collectablesManager.CreateCollectable(config);
            crystalSpawner.Grow(seed);

            TotalCrystals++;
            gameUI.UpdateTotalCrystalAmount(TotalCrystals / (float) matchHandler.MatchConfig.Mode.maxCrystals);

            if (TotalCrystals == matchHandler.MatchConfig.Mode.maxCrystals &&
                Owner.IsLocalPlayer)
            {
                photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient);
            }
        }

        #region Events
        private void OnItemCollected(ItemCollectedByPlayerMsg msg)
        {
            switch (msg.itemId)
            {
                case "hunted_collectable":
                    if (!Owner.IsLocalPlayer)
                        break;
                    crystalAmmo = Mathf.Clamp(crystalsPerCollectable + crystalAmmo, 0, maxCrystals);
                    gameUI.UpdateCrystalAmmoBar(crystalAmmo / (float)maxCrystals);
                    break;

                case "collectable_crystal":

                    TotalCrystals--;
                    gameUI.UpdateTotalCrystalAmount(TotalCrystals / matchHandler.MatchConfig.Mode.maxCrystals);
                    break;
            }
        }
        #endregion
    }
}