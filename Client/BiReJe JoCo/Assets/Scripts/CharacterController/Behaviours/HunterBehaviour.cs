using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HunterBehaviour : BaseBehaviour
    {
        [Header("Settings")]
        [SerializeField] Transform cameraRoot;
        [SerializeField] Transform fpsSetup;

        [Header("Mechanics")]
        [SerializeField] ShockMechanic shockMechanic;
        [SerializeField] TrapMechanic trapMechanic;
        [SerializeField] PingMechanic pingMechanic;
        public bool isHitting => shockMechanic.isHitting.GetValue();
  
        #region Initialization
        protected override void OnBehaviourInitialized()
        {
            if (Owner.IsLocalPlayer)
            {
                ConnectEvents();
                SetupPerspective();
            }
        }

        private void SetupPerspective()
        {
            Camera.main.transform.SetParent(cameraRoot);
            Camera.main.transform.position = cameraRoot.position;
            Camera.main.transform.rotation = cameraRoot.rotation;

            fpsSetup.SetParent(cameraRoot);
        }

        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents() 
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);

            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
            if (localPlayer.PlayerCharacter)
            {
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootHold -= OnShootHold;
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootReleased -= OnShootReleased;
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed -= OnSpecial1Pressed;
            }
          
        }
        #endregion

        #region Mechanics
        private void OnShootHold(float duration)
        {
            shockMechanic.Shoot();
        }
        private void OnShootReleased()
        {
            shockMechanic.StopShooting();
        }

        private void OnThrowTrapPressed()
        {
            trapMechanic.ThrowTrap();
        }

        private void OnSpecial1Pressed()
        {
            pingMechanic.SpawnPing();
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootHold += OnShootHold;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootReleased += OnShootReleased;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed += OnSpecial1Pressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onThrowTrapPressed += OnThrowTrapPressed;
        }
        #endregion
    }
}