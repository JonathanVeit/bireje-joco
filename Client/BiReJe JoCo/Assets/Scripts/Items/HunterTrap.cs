using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Items
{
    public class HunterTrap : LocalTrigger, IPlayerObserved
    {
        [Header("Trap Settings")]
        public Rigidbody rigidBody;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

        }

        protected override void SetupAsActive()
        {
            if (!Owner.IsLocalPlayer)
            {
                tickSystem.Unregister(this);
            }
            else
            {

            }
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            DisconnectEvents();

            foreach (var floaty in floaties)
            {
                if (floaty.Value)
                    floaty.Value.RequestDestroyFloaty();
            }

            messageHub.ShoutMessage<PlayerCollectedTrapMsg>(this);
            Destroy(this.gameObject);
        }
    }
}