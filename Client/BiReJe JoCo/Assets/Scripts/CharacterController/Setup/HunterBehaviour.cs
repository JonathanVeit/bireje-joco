using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;
using JoVei.Base.UI;
using JoVei.Base.Helper;

namespace BiReJeJoCo.Character
{
    public class HunterBehaviour : TickBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] Transform cameraRoot;
        [SerializeField] Transform fpsSetup;
        [SerializeField] Timer pingDurationTimer;
        [SerializeField] Timer pingCooldownTimer;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        private SyncVar<Vector3> pingPosition = new SyncVar<Vector3>(3);
        private GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private HunterPingFloaty pingFloaty; 

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (Owner.IsLocalPlayer)
            {
                ConnectEvents();
                SetupPerspective();
            }
            else if (localPlayer.Role == PlayerRole.Hunter)
            {
                pingPosition.OnValueReceived += OnPingUpdated;
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
       
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(pingPosition);
            if (localPlayer.PlayerCharacter)
                localPlayer.PlayerCharacter.controllerSetup.characterInput.onSpecial1Pressed -= OnSpecial1Pressed;
            pingPosition.OnValueReceived -= OnPingUpdated;
        }

        private void ConnectEvents() 
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnClosEMatch);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);

            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        private void OnSpecial1Pressed() 
        {
            if (pingCooldownTimer.State == TimerState.Counting) return;

            pingPosition.SetValue(transform.position + Vector3.up);
            pingPosition.ForceSend();
            pingCooldownTimer.Start(
                () => // update
                {
                    gameUI.UpdatePingCooldown(pingCooldownTimer.RelativeProgress);
                },null);
        }

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.controllerSetup.characterInput.onSpecial1Pressed += OnSpecial1Pressed;
        }

        private void OnPingUpdated(Vector3 position)
        {
            if (position.magnitude == 0) return;

            if (pingDurationTimer.State == TimerState.Counting)
                pingDurationTimer.Stop(true);

            var parent = uiManager.GetInstanceOf<GameUI>().floatingElementGrid;
            var target = new GameObject("ping_target");
            target.transform.position = position;
            var config = new FloatingElementConfig("hunter_ping", parent, target.transform);
            pingFloaty = floatingManager.GetElementAs<HunterPingFloaty>(config);
            pingFloaty.SetClamped();

            pingDurationTimer.Start(
                () => // update  
                {
                    if (pingFloaty)
                    {
                        pingFloaty.SetAlpha(1 - pingDurationTimer.RelativeProgress);
                    }
                },
                () => // finish
                {
                    if (pingFloaty)
                    {
                        pingFloaty.RequestDestroyFloaty();
                        Destroy(target);
                    }
                });
        }

        private void OnClosEMatch(PhotonMessage obj)
        {
            pingDurationTimer.Stop();
            pingCooldownTimer.Stop();

            if (pingFloaty)
                pingFloaty.RequestDestroyFloaty();
        }
        #endregion
    }
}