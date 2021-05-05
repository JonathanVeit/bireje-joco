using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Map
{
    public class TriggeredTeleport : LocalTrigger
    {
        [Header("Teleport Settings")]
        [SerializeField] Transform targetLocation;

        protected override void OnTriggerInteracted(byte pointId)
        {
            playerTransform.position = targetLocation.position;
        }
    }
}
