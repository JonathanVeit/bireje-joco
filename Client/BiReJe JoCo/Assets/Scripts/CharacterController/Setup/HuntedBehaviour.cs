using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using System;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HuntedBehaviour : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] float maxHealth = 100f;
        [SerializeField] string startTransformationItem;
        [SerializeField] Timer transformationDurationTimer;
        [SerializeField] Timer transformationCooldownTimer;
        [Space(10)]
        [SerializeField] float speedUpMultiplier = 1.2f;
        [SerializeField] Timer speedUpDurationTimer;
        [SerializeField] Timer speedUpCooldownTimer;

        public float Health { get; private set; }
        public Player Owner { get; private set; }

        private SyncVar<bool> isTransformed = new SyncVar<bool>(1, false);
        private string scannedItemId;
        private bool wasKilled;
        private GameObject transformedItem;

        GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private Func<bool> isGrounded;
        private int collectedItems;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            Owner = controller.Player;

            Health = maxHealth;
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
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            messageHub.RegisterReceiver<HuntedScannedItemMsg>(this, OnScannedItem);
            messageHub.RegisterReceiver<ItemCollectedByPlayerMsg>(this, OnItemCollected);
            photonMessageHub.RegisterReceiver<HuntedHitByBulletPhoMsg>(this, OnHitByBullet);
        }

        void DisconnectEvents() 
        {
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
            if (!isGrounded() || string.IsNullOrEmpty(scannedItemId))
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

            var tmp = localPlayer.PlayerCharacter.controllerSetup.walkController.movementSpeed;
            localPlayer.PlayerCharacter.controllerSetup.walkController.movementSpeed *= speedUpMultiplier;
            
            speedUpDurationTimer.Start(
                () => // update 
                {
                    gameUI.UpdateSpeedUpBar(1 - speedUpDurationTimer.RelativeProgress);
                }, 
                () => // finish
                {
                    localPlayer.PlayerCharacter.controllerSetup.walkController.movementSpeed = tmp;

                    speedUpCooldownTimer.Start(() => // update 
                    {
                        gameUI.UpdateSpeedUpBar(speedUpCooldownTimer.RelativeProgress);
                    }, null);
                });
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.controllerSetup.characterInput.onShootPressed += OnShootPressed;
            localPlayer.PlayerCharacter.controllerSetup.characterInput.onSpecial2Pressed += OnSpeedUpPressed;
            var mover = localPlayer.PlayerCharacter.controllerSetup.characterRoot.GetComponent<Mover>();
            isGrounded = () => mover.IsGrounded();
        }

        void OnHitByBullet(PhotonMessage msg) 
        {
            var casted = msg as HuntedHitByBulletPhoMsg;

            Health -= casted.dmg;
            uiManager.GetInstanceOf<GameUI>().UpdateHealthBar(Health / 100);

            if (Health <= 0 && !wasKilled)
            {
                Health = 0;
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
                    Owner.PlayerCharacter.controllerSetup.modelRoot.gameObject.SetActive(false);
                    Owner.PlayerCharacter.controllerSetup.mainCollider.enabled = false;
                    break;

                // transformed back
                case false:
                    Owner.PlayerCharacter.controllerSetup.modelRoot.gameObject.SetActive(true);
                    Owner.PlayerCharacter.controllerSetup.mainCollider.enabled = true;
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