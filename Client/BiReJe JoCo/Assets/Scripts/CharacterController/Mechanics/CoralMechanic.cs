using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CoralMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Transform spawnPoint;
        [SerializeField] int maxCrystals;
        [SerializeField] int crystalsPerCollectable;
        [SerializeField] string collectableId = "collectable_coral";

        [Header("Runtime")]
        [SerializeField] int crystalAmmo = 0;
        [SerializeField] int totalCorals;

        public int TotalCorals => totalCorals;

        private SyncVar<object[]> onSpawnCrystals = new SyncVar<object[]>(8, true);

        private int seed;

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

        public void SpawnCrystals() 
        {
            if (crystalAmmo == 0)
                return;

            if (!Owner.PlayerCharacter.ControllerSetup.WalkController.IsGrounded())
                return;

            var args = new object[2] 
            {
                spawnPoint.position,
                seed,
            };

            onSpawnCrystals.SetValue(args);
            SpawnCrystalsInternal(onSpawnCrystals.GetValue());

            crystalAmmo--;
            seed++;
            gameUI.UpdateCrystalAmmoBar(crystalAmmo / (float)maxCrystals);
        }

        private void SpawnCrystalsInternal(object[] args) 
        {
            var pos = (Vector3)args[0];
            var seed = (int)args[1];

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("coral_grower");
            var instance = Instantiate(prefab, pos, Quaternion.identity);
            var coralGrower = instance.GetComponent<CoralGrower>();
            coralGrower.Grow(seed);

            gameUI.UpdateTotalCoralAmount(TotalCorals / (float) matchHandler.MatchConfig.Mode.maxCorals);

            if (TotalCorals == matchHandler.MatchConfig.Mode.maxCorals &&
                Owner.IsLocalPlayer)
            {
                photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient);
            }
        }

        #region Events
        private void OnItemCreatedCollected(CollectableItemCreated msg)
        {
            switch (msg.itemId)
            {
                case "collectable_coral":
                    totalCorals++;
                    gameUI.UpdateTotalCoralAmount(TotalCorals / matchHandler.MatchConfig.Mode.maxCorals);
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
                    crystalAmmo = Mathf.Clamp(crystalsPerCollectable + crystalAmmo, 0, maxCrystals);
                    gameUI.UpdateCrystalAmmoBar(crystalAmmo / (float)maxCrystals);
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