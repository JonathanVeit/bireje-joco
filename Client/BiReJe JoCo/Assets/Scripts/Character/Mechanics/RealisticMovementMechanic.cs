using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class RealisticMovementMechanic : BaseBehaviourMechanic<HunterBehaviour>
    {
        [Header("Settings")]
        [SerializeField] [Range(0, 20)] public float fallSlowDownVelocity = 1f;
        [SerializeField] [Range(0, 1)] public float fallSlowDownStrength = 0.2f;
        [SerializeField] [Range(0, 20)] public float fallSlowDownDuration = 3f;

        private AdvancedWalkerController walkController => Behaviour.Owner.PlayerCharacter.ControllerSetup.WalkController;
        private bool isApplied;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            walkController.OnLand += OnPlayerLand;
        }

        private void DisconnectEvents()
        {
            if (Behaviour.Owner.PlayerCharacter &&
                Behaviour.Owner.PlayerCharacter.ControllerSetup.WalkController)
                walkController.OnLand -= OnPlayerLand;
        }
        #endregion

        private void OnPlayerLand(Vector3 velocity)
        {
            if (isApplied)
                return;

            if (velocity.y <= -fallSlowDownVelocity)
            {
                var multiplier = new LinearMovementModification(fallSlowDownStrength, fallSlowDownDuration);
                multiplier.OnDetermined += () =>
                {
                    isApplied = false;
                };
                walkController.AddModification(multiplier);
                isApplied = true;
            }
        }
    }
}