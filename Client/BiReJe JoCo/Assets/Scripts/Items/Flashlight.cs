using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class Flashlight : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Transform light;
        [SerializeField] float angle;
        [SerializeField] float speed;

        private SyncVar<Vector3> rotation = new SyncVar<Vector3>(2);
        
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
            
            light.transform.rotation = Quaternion.Lerp(light.transform.rotation, Quaternion.Euler (rotation.GetValue()), speed * Time.deltaTime);
        }

        protected override void OnBeforeDestroy()
        {
            syncVarHub.UnregisterSyncVar(rotation);
        }
    }
}