using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CoralMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Transform spawnPoint;
        [SerializeField] int maxCoralAmmo;
        [SerializeField] int coralsPerCollectable;
        [SerializeField] string collectableId = "collectable_coral";
        [SerializeField] float minShootDistance;

        [Header("Runtime")]
        [SerializeField] int coralAmmo = 0;
        [SerializeField] int totalCorals;

        public int TotalCorals => totalCorals;
        public bool AmmoIsFull => coralAmmo == maxCoralAmmo;

        private SyncVar<object[]> onSpawnCrystals = new SyncVar<object[]>(8, true);

        private int seed;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
            gameUI.UpdateCrystalAmmoBar(coralAmmo / (float)maxCoralAmmo);
        }
        protected override void OnInitializeRemote()
        {
            ConnectEvents();
            onSpawnCrystals.OnValueReceived += SpawnCoralsInternal;
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<CollectableItemCreated>(this, OnItemCreatedCollected);
            messageHub.RegisterReceiver<ItemCollectedByPlayerMsg>(this, OnItemCollected);
        }
        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(onSpawnCrystals);
        }
        #endregion

        public void SpawnCorals() 
        {
            if (!CanSpawn())
                return;

            var args = new object[2] 
            {
                spawnPoint.position,
                seed,
            };

            onSpawnCrystals.SetValue(args);
            SpawnCoralsInternal(onSpawnCrystals.GetValue());

            coralAmmo--;
            seed++;
            gameUI.UpdateCrystalAmmoBar(coralAmmo / (float)maxCoralAmmo);
        }
        private bool CanSpawn() 
        {
            if (coralAmmo == 0)
                return false;

            if (!Owner.PlayerCharacter.ControllerSetup.WalkController.IsGrounded())
                return false;

            foreach (var collectable in collectablesManager.GetAllCollectablesAs<DestroyableCoral>(x => x is DestroyableCoral))
            {
                if (Vector3.Distance(collectable.transform.position, transform.position) < minShootDistance)
                    return false;
            }

            return true;

        }
        private void SpawnCoralsInternal(object[] args) 
        {
            var pos = (Vector3)args[0];
            var seed = (int)args[1];

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("coral_grower");
            var instance = Instantiate(prefab, pos, Quaternion.identity);
            var coralGrower = instance.GetComponent<CoralGrower>();
            coralGrower.Grow(seed);

            if (TotalCorals == matchHandler.MatchConfig.Mode.maxCorals &&
                Owner.IsLocalPlayer)
            {
                photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient);
            }

            SpawnRandomCollectable(seed);
        }

        private void SpawnRandomCollectable(int seed) 
        {
            var rnd = new System.Random(seed);

            var freeSpawnPoints = collectablesManager.GetFreeSpawnPoints();
            var spawnConfig = new CollectableSpawnConfig()
            {
                i = "hunted_collectable",
                i2 = rnd.NextDouble().ToString(),
                s = freeSpawnPoints[rnd.Next(0, freeSpawnPoints.Length)],
            };
            collectablesManager.CreateCollectable(spawnConfig);
        }

        #region Events
        private void OnItemCreatedCollected(CollectableItemCreated msg)
        {
            switch (msg.itemId)
            {
                case "collectable_coral":
                    totalCorals++;
                    gameUI.UpdateTotalCoralAmount(TotalCorals / (float) matchHandler.MatchConfig.Mode.maxCorals);
                    break;
            }

            if (Owner.IsLocalPlayer)
            {
                if (TotalCorals == matchHandler.MatchConfig.Mode.maxCorals)
                {
                    photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient); 
                }
            }
        }
        private void OnItemCollected(ItemCollectedByPlayerMsg msg)
        {
            switch (msg.itemId)
            {
                case "hunted_collectable":
                    if (!Owner.IsLocalPlayer)
                        break;
                    coralAmmo = Mathf.Clamp(coralAmmo + coralsPerCollectable, 0, maxCoralAmmo);
                    gameUI.UpdateCrystalAmmoBar(coralAmmo / (float) maxCoralAmmo);
                    break;

                case "collectable_coral":
                    totalCorals--;
                    gameUI.UpdateTotalCoralAmount(TotalCorals / (float) matchHandler.MatchConfig.Mode.maxCorals);
                    break;
            }
        }
        #endregion
    }
}