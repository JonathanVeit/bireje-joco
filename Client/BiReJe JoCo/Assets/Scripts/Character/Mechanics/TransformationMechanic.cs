using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using System;
using UnityEngine;
using UnityEngine.Animations;

namespace BiReJeJoCo.Character
{
    public class TransformationMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] string startTransformationItem;
        [SerializeField] Timer transformationDurationTimer;
        [SerializeField] Timer transformationCooldownTimer;

        public bool IsTransformed => isTransformed.GetValue();
        public TransformableItem TransformedItem { get; private set; }
        
        private SyncVar<bool> isTransformed = new SyncVar<bool>(0, false);
        public string ScannedItemId { get; private set; }

        private Func<bool> isGrounded;
        private PositionConstraint pConstraint;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            isGrounded = () => localPlayer.PlayerCharacter.ControllerSetup.Mover.IsGrounded();
            ScannedItemId = startTransformationItem;

            transformationCooldownTimer.Start(() => // update 
            {
                gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
            }, null);
            gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(ScannedItemId));

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
            if (string.IsNullOrEmpty(ScannedItemId) ||
                Behaviour.ResistanceMechanic.IsDecreasing)
                return;

            if (isTransformed.GetValue())
            {
                TransformBack();
            }
            else if (transformationCooldownTimer.State == TimerState.Finished! && isGrounded())
            {
                TransformInto();
            }
        }

        public void TransformInto()
        {
            isTransformed.SetValue(true);
            OnChangedTransformation(isTransformed.GetValue());
            messageHub.ShoutMessage<BlockPlayerControlsMsg>(this, InputBlockState.Transformation);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey(ScannedItemId);
            var root = localPlayer.PlayerCharacter.ControllerSetup.ModelRoot;
            photonRoomWrapper.Instantiate(prefab.name, root.position + Vector3.up, root.rotation);

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
        public void TransformBack()
        {
            isTransformed.SetValue(false);
            OnChangedTransformation(isTransformed.GetValue());
            messageHub.ShoutMessage<UnblockPlayerControlsMsg>(this, InputBlockState.Free);

            photonRoomWrapper.Destroy(TransformedItem.gameObject);
            TransformedItem = null;

            gameUI.UpdateTransformationDurationBar(0);
            transformationDurationTimer.Stop();
            transformationCooldownTimer.Start(
                () => // update
                {
                    gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
                }, null);

            if (Owner.IsLocalPlayer)
                Destroy(pConstraint);
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

        public void SetTransformedItem(TransformableItem item)
        {
            TransformedItem = item;

            if (item && Owner.IsLocalPlayer)
            {
                pConstraint = Owner.PlayerCharacter.ControllerSetup.CharacterRoot.gameObject.AddComponent<PositionConstraint>();
                pConstraint.constraintActive = true;
                pConstraint.AddSource(new ConstraintSource()
                {
                    sourceTransform = item.transform,
                    weight = 1,
                });
            }
        }

        #region Events
        private void OnScannedItem(HuntedScannedItemMsg msg)
        {
            ScannedItemId = msg.itemId;
            gameUI.UpdateScannedItemIcon(SpriteMapping.GetMapping().GetElementForKey(msg.itemId));

            transformationCooldownTimer.Stop(false);
            transformationCooldownTimer.Start(
                () => // update
                {
                    gameUI.UpdateTransformationCooldownBar(transformationCooldownTimer.RelativeProgress);
                }, null);
        }
        #endregion
    }
}