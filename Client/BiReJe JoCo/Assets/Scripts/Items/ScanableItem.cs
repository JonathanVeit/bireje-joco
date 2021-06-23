using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class ScanableItem : LocalTrigger
    {
        [Header("Settings")]
        [SerializeField] string id;

        private HuntedBehaviour huntedBehaviour 
            => playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted)[0].PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>();

        protected override void OnTriggerInteracted(byte pointId)
        {
            messageHub.ShoutMessage<HuntedScannedItemMsg>(this, id);
        }
        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("scan");
        }
        protected override bool PlayerIsInArea(TriggerSetup trigger)
        {
            if (huntedBehaviour.TransformationMechanic.ScannedItemId == id) 
            {
                return false;
            }

            return base.PlayerIsInArea(trigger);
        }
    }
}