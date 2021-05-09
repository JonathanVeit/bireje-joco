using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class Gun : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] SyncVar<Vector3> playerShot = new SyncVar<Vector3>(0);
        [SerializeField] float bulletForce;
        [SerializeField] float coolDown;
        [SerializeField] float targetRange;
        [SerializeField] Transform bulletSpawn;
        [SerializeField] LayerMask targetLayer;
        
        private PlayerControlled controller;
        public Player Owner => controller.Player;

        private PlayerCharacterInput input;
        private float coolDownCounter;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller  = controller;
            input = localPlayer.PlayerCharacter.characterInput;

            if (controller.Player.IsLocalPlayer)
                input.onShootPressed += OnShootPressed;
            else
                playerShot.OnValueReceived += OnShotFired;

            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        public override void Tick(float deltaTime)
        {
            coolDownCounter = Mathf.Clamp(coolDownCounter += Time.deltaTime, 0, coolDown);
            uiManager.GetInstanceOf<GameUI>().UpdateWeaponCooldown(coolDownCounter, coolDown);
        }

        private void OnShootPressed()
        {
            if (coolDownCounter != coolDown)
            {
                return;
            }

            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, targetRange, targetLayer, QueryTriggerInteraction.Ignore))
            {
                targetPoint = hit.point;
            }
            else
                targetPoint = Camera.main.transform.position + Camera.main.transform.forward * targetRange;

            playerShot.SetValue(targetPoint);
            playerShot.ForceSend();
            OnShotFired(playerShot.GetValue());
            coolDownCounter = 0;

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red, 5f);
        }

        #region Events
        private void OnShotFired(Vector3 target)
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("default_bullet");
            var bullet = poolingManager.PoolInstance(prefab, bulletSpawn.position, Quaternion.identity);

            var dir = (target - bulletSpawn.position).normalized;
            bullet.GetComponent<Bullet>().Initialize(controller.Player.IsLocalPlayer);
            bullet.GetComponent<Rigidbody>().AddForce(dir * bulletForce, ForceMode.Impulse);
        }

        private void OnMatchClosed(PhotonMessage msg)
        {
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(playerShot);
        }
        #endregion
    }
}
