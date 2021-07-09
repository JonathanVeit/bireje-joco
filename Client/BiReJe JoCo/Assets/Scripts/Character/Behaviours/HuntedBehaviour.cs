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
        [SerializeField] CoralMechanic coralMechanic;

        #region Access
        public TransformationMechanic TransformationMechanic => transformationMechanic;
        public SpeedUpMechanic SpeedUpMechanic => speedUpMechanic;
        public ResistanceMechanic ResistanceMechanic => resistanceMechanic;
        public CoralMechanic CoralMechanic => coralMechanic;
        #endregion

        GameUI gameUI => uiManager.GetInstanceOf<GameUI>();

        #region Initialization
        protected override void OnBehaviourInitialized()
        {
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

            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootPressed -= OnShootPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpeedUpPressed -= OnSpeedUpPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpawnCoralsPressed -= OnSpawnCoralsPressed;
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

        private void OnSpawnCoralsPressed() 
        {
            coralMechanic.SpawnCorals();
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
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpeedUpPressed += OnSpeedUpPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpawnCoralsPressed += OnSpawnCoralsPressed;
        }

        private void OnHuntedCatched(PhotonMessage msg)
        {
            var castedMsg = msg as HuntedCatchedPhoMsg;
            Owner.PlayerCharacter.ControllerSetup.CharacterRoot.gameObject.SetActive(false);
        }
        #endregion
    }
}