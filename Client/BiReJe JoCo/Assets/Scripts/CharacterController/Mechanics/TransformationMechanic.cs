using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using System;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class TransformationMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] string startTransformationItem;
        [SerializeField] Timer transformationDurationTimer;
        [SerializeField] Timer transformationCooldownTimer;

        private SyncVar<bool> isTransformed = new SyncVar<bool>(0, false);
        private string scannedItemId;
        private GameObject transformedItem;

        private GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private Func<bool> isGrounded;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            isGrounded = () => localPlayer.PlayerCharacter.ControllerSetup.Mover.IsGrounded();
            scannedItemId = startTransformationItem;

            transformationCooldownTimer.Start(() => // update 
            {
                gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
            }, null);
            gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(scannedItemId));

            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
            isTransformed.OnValueReceived += OnChangedTransformation;
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<HuntedScannedItemMsg>(this, OnScannedItem);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);

            transformationDurationTimer.Stop();
            transformationCooldownTimer.Stop();

            if (syncVarHub)
            {
                syncVarHub.UnregisterSyncVar(isTransformed);
            }
        }
        #endregion

        public void ToggleTransformation()
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
            var root = localPlayer.PlayerCharacter.ControllerSetup.ModelRoot;
            transformedItem = photonRoomWrapper.Instantiate(prefab.name, root.position, root.rotation);

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

            SpawnSFX();
        }
        private void SpawnSFX()
        {
            var sfxPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_transformation_sfx");
            var sfx = poolingManager.PoolInstance(sfxPrefab, transform.parent.GetChild(0).position, transform.parent.GetChild(0).rotation);
            sfx.transform.position -= Vector3.up;
        }

        #region Events
        private void OnScannedItem(HuntedScannedItemMsg msg)
        {
            scannedItemId = msg.itemId;
            gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(msg.itemId));
        }
        #endregion
    }
}