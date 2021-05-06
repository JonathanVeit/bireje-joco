using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CharacterShooter : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] SyncVar<Vector3> playerShot = new SyncVar<Vector3>(0);
        [SerializeField] float bulletForce;
        [SerializeField] float targetRange;
        [SerializeField] Transform bulletSpawn;
        [SerializeField] LayerMask targetLayer;
        
        private PlayerControlled controller;
        public Player Owner => controller.Player;

        private PlayerCharacterInput input;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller  = controller;
            input = GetComponentInParent<PlayerCharacterInput>();

            if(controller.Player.IsLocalPlayer)
                input.onShootPressed += OnShootPressed;
            playerShot.OnValueReceived += OnShotFired;

            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);
        }

        protected override void OnBeforeDestroy()
        {
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        private void OnShootPressed()
        {
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
