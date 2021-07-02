using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using System;
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
        [SerializeField] float minAmmoRespawnDistance;

        [Header("Runtime")]
        [SerializeField] int coralAmmo = 0;
        [SerializeField] int totalCorals;

        public int TotalCorals => totalCorals;
        public bool AmmoIsFull => coralAmmo == maxCoralAmmo;
        public event Action onSpawnedCorals;

        private SyncVar<object[]> onSpawnCrystals = new SyncVar<object[]>(8, true);

        private int seed;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
            gameUI.UpdateCrystalAmmoBar(coralAmmo / (float)maxCoralAmmo);
            Owner.PlayerCharacter.ControllerSetup.AnimationController.onAnimationEvent += OnAnimationEvent;
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

            photonMessageHub.RegisterReceiver<SpawnNewCoralAmmoPhoMsg>(this, OnSpawnNewAmmoReceived);
        }
        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(onSpawnCrystals);

            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
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
            onSpawnedCorals?.Invoke();

            messageHub.ShoutMessage<BlockPlayerControlsMsg>(this, new BlockPlayerControlsMsg(InputBlockState.Transformation));
            Owner.PlayerCharacter.ControllerSetup.AnimationController.SetTrigger("place_corals");
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

            var sfxPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_place_corals");
            poolingManager.PoolInstance(sfxPrefab, transform.position, transform.rotation);
        }

        private void SpawnRandomCollectable() 
        {
            photonMessageHub.ShoutMessage<SpawnNewCoralAmmoPhoMsg>(PhotonMessageTarget.AllViaServer, GetSuitableSpawnPoint());
        }
        private int GetSuitableSpawnPoint() 
        {
            // current scene configuration
            var mapConfig = matchHandler.MatchConfig.mapConfig;

            // all free avaiable spawnpoint indices 
            var rndIndex = mapConfig.GetRandomCollectableSpawnPointIndex();

            // search for a point that is not too close to the hunteds current position
            int counter = 0;
            int maxSearchCount = 1000;
            while (true)
            {
                counter++;
                if (counter >= maxSearchCount)
                {
                    Debug.Log($"Failed to find suitable spawnpoit for coral ammo with {maxSearchCount} tries. Returns random one.");
                    break;
                }

                var distToHunted = Vector3.Distance(transform.position, mapConfig.GetCollectableSpawnPoint(rndIndex));

                foreach (var collectable in collectablesManager.AllCollectables)
                {
                    var distToCollectable = Vector3.Distance((collectable as Component).transform.position, mapConfig.GetCollectableSpawnPoint(rndIndex));

                    if (distToCollectable < matchHandler.MatchConfig.Mode.minCollectableDistance)
                        continue;
                }

                // far enough and not already used? -> return
                if (distToHunted >= minAmmoRespawnDistance && 
                    !collectablesManager.HasCollectableAtIndex(rndIndex))
                {
                    return rndIndex;
                }

                rndIndex = mapConfig.GetRandomCollectableSpawnPointIndex();
            }

            // no point found that is far enough? -> return one of the rejected ones
            return rndIndex;
        }

        #region Events
        private void OnSpawnNewAmmoReceived(PhotonMessage msg) 
        {
            var castedMsg = (SpawnNewCoralAmmoPhoMsg)msg;

            var spawnConfig = new CollectableSpawnConfig()
            {
                i = "collectable_coral_ammo",
                s = castedMsg.pointIndex,
            };
            collectablesManager.CreateCollectable(spawnConfig);
        }

        private void OnItemCreatedCollected(CollectableItemCreated msg)
        {
            switch (msg.itemId)
            {
                case "destroyable_coral":
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
                case "collectable_coral_ammo":
                    if (!Owner.IsLocalPlayer)
                        break;
                    coralAmmo = Mathf.Clamp(coralAmmo + coralsPerCollectable, 0, maxCoralAmmo);
                    gameUI.UpdateCrystalAmmoBar(coralAmmo / (float) maxCoralAmmo);
                    
                    if (Owner.IsLocalPlayer)
                        SpawnRandomCollectable();
                    break;

                case "destroyable_coral":
                    totalCorals--;
                    gameUI.UpdateTotalCoralAmount(TotalCorals / (float) matchHandler.MatchConfig.Mode.maxCorals);
                    break;
            }
        }

        private void OnAnimationEvent(string eventName)
        {
            if (eventName == "place_corals_finished")
            {
                messageHub.ShoutMessage<UnblockPlayerControlsMsg>(this, new UnblockPlayerControlsMsg(InputBlockState.Free));
            }
        }
        #endregion
    }
}