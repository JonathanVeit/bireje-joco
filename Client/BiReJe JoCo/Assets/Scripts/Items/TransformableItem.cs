using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class TransformableItem : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Rigidbody rb;
        [SerializeField] float spawnForce;
        [SerializeField] GameObject cameraRig;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            else
            {
                rb.AddForce(Vector3.up * spawnForce, ForceMode.Impulse);
                cameraRig.SetActive(true);
            }

            AssignToTransformationMechanic();
        }
        protected override void OnBeforeDestroy()
        {
            RemoveFromTransformationMechanic();
        }

        private void AssignToTransformationMechanic()
        {
            Owner.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic.SetTransformedItem(this.gameObject);
        }
        private void RemoveFromTransformationMechanic()
        {
            Owner.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic.SetTransformedItem(null);
        }
    }
}