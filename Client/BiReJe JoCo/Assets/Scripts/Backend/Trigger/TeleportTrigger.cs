using UnityEngine;

namespace BiReJeJoCo.Backend
{
    public class TeleportTrigger : LocalTrigger
    {
        [Header("Teleport Trigger Settings")]
        [SerializeField] Transform teleportTarget;

        protected override void OnTriggerInteracted()
        {
            playerTransform.position = teleportTarget.position;
        }
    }
}
