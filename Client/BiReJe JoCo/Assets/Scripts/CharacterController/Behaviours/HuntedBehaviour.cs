using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using JoVei.Base;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HuntedBehaviour : BaseBehaviour, ITickable
    {
        [Header("Mechanics")]
        [SerializeField] TransformationMechanic transformationMechanic;
        [SerializeField] SpeedUpMechanic speedUpMechanic;
        [SerializeField] ResistanceMechanic resistanceMechanic;
        [SerializeField] CrystalMechanic crystalMechanic;

        #region Access
        public TransformationMechanic TransformationMechanic => transformationMechanic;
        public SpeedUpMechanic SpeedUpMechanic => speedUpMechanic;
        public ResistanceMechanic ResistanceMechanic => resistanceMechanic;
        public CrystalMechanic CrystalMechanic => crystalMechanic;
        #endregion

        GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private int collectedItems;

        #region Initialization
        protected override void OnBehaviourInitialized()
        {
            gameUI.UpdateCrystalAmmoBar(1);
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        void ConnectEvents()
        {
            if (Owner.IsLocalPlayer)
            {
                tickSystem.Register(this, "update");
                messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            }

            photonMessageHub.RegisterReceiver<HuntedCatchedPhoMsg>(this, OnHuntedCatched);
        }
        void DisconnectEvents()
        {
            tickSystem.Unregister(this);
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Mechanics
        private void OnShootPressed()
        {
            transformationMechanic.ToggleTransformation();
        }

        private void OnSpeedUpPressed()
        {
            speedUpMechanic.UseSpeed();
        }

        private void OnThrowTrapPressed() 
        {
            crystalMechanic.SpawnCrystals();
        }

        public void Tick(float deltaTime)
        {
            if (Owner.IsLocalPlayer)
                resistanceMechanic.UpdateResistance();
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootPressed += OnShootPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed += OnSpeedUpPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onThrowTrapPressed += OnThrowTrapPressed;
        }

        private void OnHuntedCatched(PhotonMessage msg)
        {
            var castedMsg = msg as HuntedCatchedPhoMsg;
            Owner.PlayerCharacter.ControllerSetup.CharacterRoot.gameObject.SetActive(false);
        }
        #endregion
    }
}