using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Owner.PlayerCharacter.ControllerSetup.AnimationController.onPlaceSporesFinished += OnFinishedPlaceCorals;
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

            SpawnRandomCollectable(seed);

            var sfxPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_place_corals");
            poolingManager.PoolInstance(sfxPrefab, transform.position, transform.rotation);
        }

        private void SpawnRandomCollectable(int seed) 
        {
            var rnd = new System.Random(seed);

            var spawnConfig = new CollectableSpawnConfig()
            {
                i = "collectable_coral_ammo",
                i2 = rnd.NextDouble().ToString(),
                s = GetSuitableSpawnPoint(rnd),
            };
            collectablesManager.CreateCollectable(spawnConfig);
        }
        private int GetSuitableSpawnPoint(System.Random rnd) 
        {
            // all free avaiable spawnpoint indices 
            var freeSpawnPointsIndices = collectablesManager.GetFreeSpawnPoints().ToList();
            var rejectedSpawnPointIndices = new List<int>();

            // current scene configuration
            var sceneConfig = MapConfigMapping.GetMapping().GetElementForKey(matchHandler.MatchConfig.Mode.gameScene);

            // search for a point that is not too close to the hunteds current position
            while (freeSpawnPointsIndices.Count > 0)
            {
                var rndIndex = freeSpawnPointsIndices[rnd.Next(0, freeSpawnPointsIndices.Count)];
                var distToHunted = Vector3.Distance(transform.position, sceneConfig.GetCollectableSpawnPoint(rndIndex));

                // far enough? -> return
                if (distToHunted >= minAmmoRespawnDistance)
                {
                    return rndIndex;
                }
                // too close? -> add to rejected 
                rejectedSpawnPointIndices.Add(rndIndex);
                freeSpawnPointsIndices.Remove(rndIndex);
            }

            // no point found that is far enough? -> return one of the rejected ones
            return freeSpawnPointsIndices[rnd.Next(0, rejectedSpawnPointIndices.Count)];
        }

        #region Events
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
                    break;

                case "destroyable_coral":
                    totalCorals--;
                    gameUI.UpdateTotalCoralAmount(TotalCorals / (float) matchHandler.MatchConfig.Mode.maxCorals);
                    break;
            }
        }

        private void OnFinishedPlaceCorals()
        {
            messageHub.ShoutMessage<UnblockPlayerControlsMsg>(this, new UnblockPlayerControlsMsg(InputBlockState.Free));
        }
        #endregion
    }
}