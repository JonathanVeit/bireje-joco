using BiReJeJoCo.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BiReJeJoCo.Character
{
    public class HunterBehaviour : BaseBehaviour
    {
        [Header("Settings")]
        [SerializeField] Transform cameraRoot;
        [SerializeField] Transform fpsSetup;
        [SerializeField] Vector3 fpsSetupPosition;

        [Header("Mechanics")]
        [SerializeField] ShockMechanic shockMechanic;
        [SerializeField] TrapMechanic trapMechanic;
        [SerializeField] PingMechanic pingMechanic;

        #region Access
        public ShockMechanic ShockMechanic => shockMechanic;
        public TrapMechanic TrapMechanic => trapMechanic;
        public PingMechanic PingMechanic => pingMechanic;
        #endregion
  
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
            fpsSetup.transform.localPosition = fpsSetupPosition;
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
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpawnPingPressed -= OnSpawnPingPresed;
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
        private void OnReload() 
        {
            shockMechanic.Reload();
        }

        private void OnThrowTrapPressed()
        {
            trapMechanic.ThrowTrap();
        }

        private void OnSpawnPingPresed()
        {
            pingMechanic.SpawnPing();
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootHold += OnShootHold;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootReleased += OnShootReleased;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onReloadPressed += OnReload;

            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpawnPingPressed += OnSpawnPingPresed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onThrowTrapPressed += OnThrowTrapPressed;
        }
        #endregion
    }
}