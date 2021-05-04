using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Map
{
    public class TeleportTrigger : LocalTrigger
    {
        [Header("Teleport Trigger Settings")]
        [SerializeField] Transform teleportTarget;

        protected override void OnTriggerInteracted(byte pointId)
        {
            playerTransform.position = teleportTarget.position;
        }
    }
}
