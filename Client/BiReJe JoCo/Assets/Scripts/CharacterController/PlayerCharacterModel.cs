using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class PlayerCharacterModel : SystemBehaviour, IPlayerObserved
    {
        public float Health { get; private set; } = 100f;
        public Player Owner { get; private set; }

        private bool wasKilled;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            Owner = controller.Player;
            ConnectEvents();
        }

        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        void ConnectEvents()
        {
            if (Owner.Role == PlayerRole.Hunted)
                photonMessageHub.RegisterReceiver<HuntedHitByBulletPhoMsg>(this, OnHuntedHitByBullet);
        }

        void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        void OnHuntedHitByBullet(PhotonMessage msg) 
        {
            var casted = msg as HuntedHitByBulletPhoMsg;

            Health -= casted.dmg;

            if (Health <= 0 && !wasKilled)
            {
                Health = 0;
                photonMessageHub.ShoutMessage<HuntedKilledPhoMsg>(PhotonMessageTarget.MasterClient);
                wasKilled = true;
            }
        }
        #endregion
    }
}