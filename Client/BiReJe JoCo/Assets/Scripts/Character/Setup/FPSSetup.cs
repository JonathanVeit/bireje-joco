using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Items
{
    public class FPSSetup : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Transform sourceTransform;
        [SerializeField] Transform rootTransform;
        [SerializeField] Transform targetIK;
        [SerializeField] float rotationSpeed;
        [SerializeField] float maxAngle;
        [SerializeField] Light flashlight;
        [SerializeField] SyncVar<Vector3> rotation = new SyncVar<Vector3>(5);
        [SerializeField] SyncVar<bool> flashLightIsOn = new SyncVar<bool>(6, true, true);

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
            {
                flashLightIsOn.OnValueReceived += (x) =>
                {
                    flashlight.enabled = x;
                    soundEffectManager.Play("hunter_flashlight_click", flashlight.transform.position);
                };
            }
            else
            {
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onToggleFlashlightPressed += ToggleFlashlight;
            }
        }

        public override void Tick(float deltaTime)
        {
            if (Camera.main == null) return;

            if (Owner.IsLocalPlayer)
            { 
                rotation.SetValue(sourceTransform.rotation.eulerAngles);
            } 
            else
            {
                targetIK.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.Euler (rotation.GetValue()), maxAngle);
            }
        }
        private void ToggleFlashlight() 
        {
            flashlight.enabled = !flashlight.enabled;
            flashLightIsOn.SetValue(flashlight.enabled);
            soundEffectManager.Play("hunter_flashlight_click", flashlight.transform.position);
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