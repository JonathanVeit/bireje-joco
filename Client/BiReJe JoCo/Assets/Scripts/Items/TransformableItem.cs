using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class TransformableItem : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Rigidbody rb;
        [SerializeField] float spawnForce;

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
                Debug.Log(Vector3.up * spawnForce);
                rb.AddForce(Vector3.up * spawnForce, ForceMode.Impulse);
            }
        }
    }
}