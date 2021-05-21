using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Items
{
    public class FPSSetup : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Transform target;
        [SerializeField] float speed;
        [SerializeField] SyncVar<Vector3> rotation = new SyncVar<Vector3>(2);
        
        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
        }

        public override void Tick(float deltaTime)
        {
            if (Camera.main == null) return;

            if (Owner.IsLocalPlayer)
                rotation.SetValue(Camera.main.transform.rotation.eulerAngles);

            target.rotation = Quaternion.Lerp(target.rotation, Quaternion.Euler (rotation.GetValue()), speed * Time.deltaTime);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();

            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(rotation);
        }
    }
}