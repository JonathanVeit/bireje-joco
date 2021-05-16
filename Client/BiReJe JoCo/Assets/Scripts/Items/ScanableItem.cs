using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class ScanableItem : LocalTrigger
    {
        [Header("Settings")]
        [SerializeField] string id;

        protected override void OnTriggerInteracted(byte pointId)
        {
            messageHub.ShoutMessage<HuntedScannedItemMsg>(this, id);
        }
        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("scan");
        }
    }
}