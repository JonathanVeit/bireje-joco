using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HuntedBehaviour : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float transformationDuration = 6;
        [SerializeField] float transformationCooldownDuration = 12;

        public float Health { get; private set; }
        public Player Owner { get; private set; }

        private SyncVar<bool> isTransformed = new SyncVar<bool>(1, false);
        private bool wasKilled;
        private GameObject transformedItem;

        float transformationCounter = 0;
        float transformationCooldownCounter = 0;


        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            Owner = controller.Player;
            ConnectEvents();
            Health = maxHealth;
            uiManager.GetInstanceOf<GameUI>().UpdateHealthBar(Health, 100);

            if (!Owner.IsLocalPlayer)
            {
                isTransformed.OnValueReceived += OnChangedTransformation;
            }
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();

            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(isTransformed);
        }

        void ConnectEvents()
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            photonMessageHub.RegisterReceiver<HuntedHitByBulletPhoMsg>(this, OnHitByBullet);
        }

        void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Transformation
        private void OnShootPressed()
        {
            if (isTransformed.GetValue())
            {
                TransformBack();
            }
            else if (transformationCooldownCounter >= transformationCooldownDuration)
            {
                TransformInto();
            }
        }

        private void TransformInto()
        {
            isTransformed.SetValue(true);
            OnChangedTransformation(true);
            messageHub.ShoutMessage<BlockPlayerControlsMsg>(this, InputBlockState.Transformation);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_fake_model");
            transformedItem = photonRoomWrapper.Instantiate(prefab.name, transform.parent.GetChild(0).position, transform.parent.GetChild(0).rotation);

            uiManager.GetInstanceOf<GameUI>().UpdateTransformationCooldownBar(0, transformationCooldownDuration);
        }
        private void TransformBack()
        {
            isTransformed.SetValue(false);
            OnChangedTransformation(false);
            messageHub.ShoutMessage<UnblockPlayerControlsMsg>(this, InputBlockState.Free);

            photonRoomWrapper.Destroy(transformedItem);
            transformedItem = null;

            transformationCounter = 0;
            transformationCooldownCounter = 0;
            uiManager.GetInstanceOf<GameUI>().UpdateTransformationDurationBar(0, transformationDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (!Owner.IsLocalPlayer)
                return;

            if (isTransformed.GetValue())
            {
                uiManager.GetInstanceOf<GameUI>().UpdateTransformationDurationBar(transformationDuration - transformationCounter, transformationDuration);
                transformationCounter += Time.deltaTime;

                if (transformationCounter >= transformationDuration)
                {
                    TransformBack();
                }
            }
            else
            {
                uiManager.GetInstanceOf<GameUI>().UpdateTransformationCooldownBar(transformationCooldownCounter, transformationCooldownDuration);
                transformationCooldownCounter = Mathf.Clamp(transformationCooldownCounter + Time.deltaTime, 0, transformationCooldownDuration);
            }
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.characterInput.onShootPressed += OnShootPressed;
        }

        void OnHitByBullet(PhotonMessage msg) 
        {
            var casted = msg as HuntedHitByBulletPhoMsg;

            Health -= casted.dmg;
            uiManager.GetInstanceOf<GameUI>().UpdateHealthBar(Health, 100);

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
                    foreach (Transform curChild in  transform.parent.GetChild(0))
                        curChild.gameObject.SetActive(false);
                    GetComponentInParent<Collider>().enabled = false;
                    break;

                // transformed back
                case false:
                    foreach (Transform curChild in transform.parent.GetChild(0))
                        curChild.gameObject.SetActive(true);
                    GetComponentInParent<Collider>().enabled = true;
                    break;
            }

            var sfxPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_transformation_sfx");
            var sfx = poolingManager.PoolInstance(sfxPrefab, transform.parent.GetChild(0).position, transform.parent.GetChild(0).rotation);
            sfx.transform.position -= Vector3.up;
        }
        #endregion
    }
}