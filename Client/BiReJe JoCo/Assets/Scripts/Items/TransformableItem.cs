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
        [SerializeField] Transform lookTransforms;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public Transform LookTransforms => lookTransforms;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                return;
            }
            rb.AddForce(Vector3.up * spawnForce, ForceMode.Impulse);
            cameraRig.SetActive(true);

            AssignToTransformationMechanic();
        }
        protected override void OnBeforeDestroy()
        {
            RemoveFromTransformationMechanic();
        }

        private void AssignToTransformationMechanic()
        {
            if (Owner.PlayerCharacter)
                Owner.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic.SetTransformedItem(this);
        }
        private void RemoveFromTransformationMechanic()
        {
            if (Owner.PlayerCharacter)
                Owner.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic.SetTransformedItem(null);
        }
    }
}