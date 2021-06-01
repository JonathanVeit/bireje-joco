using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Items
{
    public class FPSSetup : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Transform target;
        [SerializeField] float speed;
        [SerializeField] Light flashlight;
        [SerializeField] SyncVar<Vector3> rotation = new SyncVar<Vector3>(5);
        [SerializeField] SyncVar<bool> flashLightIsOn = new SyncVar<bool>(6);

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
                flashLightIsOn.OnValueReceived += (x) => flashlight.enabled = x;
            else
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onToggleFlashlight += ToggleFlashlight;
        }

        public override void Tick(float deltaTime)
        {
            if (Camera.main == null) return;

            if (Owner.IsLocalPlayer)
                rotation.SetValue(Camera.main.transform.rotation.eulerAngles);

            target.rotation = Quaternion.Lerp(target.rotation, Quaternion.Euler (rotation.GetValue()), speed * Time.deltaTime);
        }
        private void ToggleFlashlight() 
        {
            flashlight.enabled = !flashlight.enabled;
            flashLightIsOn.SetValue(flashlight.enabled);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();

            if (syncVarHub)
            {
                syncVarHub.UnregisterSyncVar(rotation);
                syncVarHub.UnregisterSyncVar(flashLightIsOn);
            }
        }
    }
}