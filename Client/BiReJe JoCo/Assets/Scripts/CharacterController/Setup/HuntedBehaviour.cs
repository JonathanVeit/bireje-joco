using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using JoVei.Base;
using JoVei.Base.Helper;
using System;
using System.Linq;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HuntedBehaviour : BaseBehaviour, ITickable
    {
        [Header("Settings")]
        [SerializeField] float maxResistance = 100f;
        [SerializeField] float minResistance = 20f;
        [SerializeField] float resistanceRegenerationRate = 1;
        [SerializeField] float resistanceLossRate = 1;
        [SerializeField] [Range(0, 1)] float maxResistanceSlowdown = 1;

        [Space(10)]
        [SerializeField] string startTransformationItem;
        [SerializeField] Timer transformationDurationTimer;
        [SerializeField] Timer transformationCooldownTimer;

        [Space(10)]
        [SerializeField] float speedUpMultiplier = 1.2f;
        [SerializeField] Timer speedUpDurationTimer;
        [SerializeField] Timer speedUpCooldownTimer;

        public float Resistance { get; private set; }

        private SyncVar<bool> isTransformed = new SyncVar<bool>(0, false);
        private string scannedItemId;
        private bool wasKilled;
        private GameObject transformedItem;

        GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private Func<bool> isGrounded;
        private int collectedItems;

        private bool isHitted;
        private MovementMultiplier hitMultiplier;

        #region Initialization
        protected override void OnBehaviourInitialized()
        {
            Resistance = maxResistance;
            gameUI.UpdateHealthBar(1);

            if (!Owner.IsLocalPlayer)
            {
                isTransformed.OnValueReceived += OnChangedTransformation;
            }
            else
            {
                ConnectEvents(); 
                transformationCooldownTimer.Start(() => // update 
                {
                    gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
                }, null);
                speedUpCooldownTimer.Start(() => // update 
                {
                    gameUI.UpdateSpeedUpBar(speedUpCooldownTimer.RelativeProgress);
                }, null);

                scannedItemId = startTransformationItem;
                gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(scannedItemId));
            }
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            transformationDurationTimer.Stop();
            transformationCooldownTimer.Stop();

            speedUpDurationTimer.Stop();
            speedUpCooldownTimer.Stop();
            DisconnectEvents();
        }

        void ConnectEvents()
        {
            tickSystem.Register(this, "update");
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            messageHub.RegisterReceiver<HuntedScannedItemMsg>(this, OnScannedItem);
            messageHub.RegisterReceiver<ItemCollectedByPlayerMsg>(this, OnItemCollected);
            photonMessageHub.RegisterReceiver<HuntedHitByBulletPhoMsg>(this, OnHitByBullet);
        }
        void DisconnectEvents() 
        {
            tickSystem.Unregister(this);
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);

            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(isTransformed);
        }
        #endregion

        #region Transformation
        private void OnShootPressed()
        {
            if (!isGrounded() || string.IsNullOrEmpty(scannedItemId) ||
                isHitted)
                return;

            if (isTransformed.GetValue())
            {
                TransformBack();
            }
            else if (transformationCooldownTimer.State == TimerState.Finished)
            {
                TransformInto();
            }
        }

        private void TransformInto()
        {
            isTransformed.SetValue(true);
            OnChangedTransformation(isTransformed.GetValue());
            messageHub.ShoutMessage<BlockPlayerControlsMsg>(this, InputBlockState.Transformation);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(scannedItemId);
            transformedItem = photonRoomWrapper.Instantiate(prefab.name, transform.parent.GetChild(0).position, transform.parent.GetChild(0).rotation);

            gameUI.UpdateTransformationCooldownBar(0);
            transformationDurationTimer.Start(
            () => // update 
            {
                gameUI.UpdateTransformationDurationBar(1 - transformationDurationTimer.RelativeProgress);
            }, 
            () => // finish
            { 
                TransformBack(); 
            });
        }
        private void TransformBack()
        {
            isTransformed.SetValue(false);
            OnChangedTransformation(isTransformed.GetValue());
            messageHub.ShoutMessage<UnblockPlayerControlsMsg>(this, InputBlockState.Free);

            photonRoomWrapper.Destroy(transformedItem);
            transformedItem = null;

            gameUI.UpdateTransformationDurationBar(0);
            transformationDurationTimer.Stop();
            transformationCooldownTimer.Start(
                () => // update
            {
                gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
            }, null);
        }
        #endregion

        #region Speed Up
        private void OnSpeedUpPressed() 
        {
            if (speedUpCooldownTimer.State != TimerState.Finished ||
                speedUpDurationTimer.State == TimerState.Counting) return;

            var multiplier = new MovementMultiplier(speedUpMultiplier);
            localPlayer.PlayerCharacter.ControllerSetup.WalkController.AddMultiplier(multiplier);

            speedUpDurationTimer.Start(
                () => // update 
                {
                    gameUI.UpdateSpeedUpBar(1 - speedUpDurationTimer.RelativeProgress);
                }, 
                (Action)(() => // finish
                {
                    localPlayer.PlayerCharacter.ControllerSetup.WalkController.RemoveMultiplier(multiplier);

                    speedUpCooldownTimer.Start(() => // update 
                    {
                        gameUI.UpdateSpeedUpBar(speedUpCooldownTimer.RelativeProgress);
                    }, null);
                }));
        }
        #endregion

        #region Resistance 
        public void Tick(float deltaTime)
        {
            var allHunter = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunter).ToList();
            if (allHunter.Count == 0) return;

            int hittingHunter = 0;
            foreach (var hunter in allHunter)
            {
                if (hunter.PlayerCharacter == null) continue;

                if (hunter.PlayerCharacter.ControllerSetup.GetBehaviourAs<HunterBehaviour>().isHitting.GetValue())
                    hittingHunter++;
            }

            if (hittingHunter == 0)
            {
                Resistance = Mathf.MoveTowards(Resistance, maxResistance, resistanceRegenerationRate * Time.deltaTime);
                isHitted = false;
            }
            else
            {
                float hitPercentage = hittingHunter / allHunter.Count;
                Resistance = Mathf.MoveTowards(Resistance, minResistance, resistanceLossRate * hitPercentage * Time.deltaTime);

                isHitted = true;
                if (isTransformed.GetValue())
                {
                    TransformBack();
                }
            }

            var percentage = Mathf.InverseLerp(minResistance, maxResistance, Resistance); // -> 0
            var negPercentage = 1 - percentage;  // -> 1

            hitMultiplier.Set(1 - (negPercentage * maxResistanceSlowdown));
            uiManager.GetInstanceOf<GameUI>().UpdateHitOverlay(negPercentage);
            Debug.Log(Resistance);
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootPressed += OnShootPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed += OnSpeedUpPressed;
            
            var mover = localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.GetComponent<Mover>();
            isGrounded = () => mover.IsGrounded();

            hitMultiplier = new MovementMultiplier(1);
            localPlayer.PlayerCharacter.ControllerSetup.WalkController.AddMultiplier(hitMultiplier);
        }

        void OnHitByBullet(PhotonMessage msg) 
        {
            uiManager.GetInstanceOf<GameUI>().UpdateHealthBar(Resistance / 100);

            if (Resistance <= 0 && !wasKilled)
            {
                Resistance = 0;
                photonMessageHub.ShoutMessage<HuntedKilledPhoMsg>(PhotonMessageTarget.MasterClient);
                wasKilled = true;
            }

            if (isTransformed.GetValue())
            {
                TransformBack();
            }
        }

        void OnChangedTransformation(bool isTransformed) 
        {
            switch (isTransformed)
            {
                // transformed into
                case true:
                    Owner.PlayerCharacter.ControllerSetup.ModelRoot.gameObject.SetActive(false);
                    Owner.PlayerCharacter.ControllerSetup.MainCollider.enabled = false;
                    break;

                // transformed back
                case false:
                    Owner.PlayerCharacter.ControllerSetup.ModelRoot.gameObject.SetActive(true);
                    Owner.PlayerCharacter.ControllerSetup.MainCollider.enabled = true;
                    break;
            }

            var sfxPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_transformation_sfx");
            var sfx = poolingManager.PoolInstance(sfxPrefab, transform.parent.GetChild(0).position, transform.parent.GetChild(0).rotation);
            sfx.transform.position -= Vector3.up;
        }

        private void OnScannedItem(HuntedScannedItemMsg msg)
        {
            scannedItemId = msg.itemId;
            gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(msg.itemId));
        }

        void OnItemCollected(ItemCollectedByPlayerMsg msg)
        {
            collectedItems++;
            gameUI.UpdateCollectedItemAmount(collectedItems);

            if (collectedItems >= matchHandler.MatchConfig.Mode.huntedCollectables)
            {
                photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient);
            }
        }
        #endregion
    }
}