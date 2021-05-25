using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Map
{
    public class TriggeredTeleport : LocalTrigger
    {
        [Header("Teleport Settings")]
        [SerializeField] Transform targetLocation;

        protected override void OnTriggerInteracted(byte pointId)
        {
            throw new System.Exception("lol");
            playerTransform.position = targetLocation.position;
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("Teleport");
        }
    }
}
