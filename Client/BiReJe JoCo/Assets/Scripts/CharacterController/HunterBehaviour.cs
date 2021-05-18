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
        [SerializeField] Timer pingDurationTimer;
        [SerializeField] Timer pingCooldownTimer;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        private SyncVar<Vector3> pingPosition = new SyncVar<Vector3>(3);

        private GameUI gameUI => uiManager.GetInstanceOf<GameUI>();

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (Owner.IsLocalPlayer)
            {
                ConnectEvents();
            }
            else if (localPlayer.Role == PlayerRole.Hunter)
            {
                pingPosition.OnValueReceived += OnPingUpdated;
            }
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
            pingDurationTimer.Stop();
            pingCooldownTimer.Stop();

            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(pingPosition);
        }

        private void ConnectEvents() 
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
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
                    gameUI.UpdatePingCooldown(pingCooldownTimer.Progress, pingCooldownTimer.Duration);
                },null);
        }

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.characterInput.onSpecial1Pressed += OnSpecial1Pressed;
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
            var floaty = floatingManager.GetElement(config) as HunterPingFloaty;
            floaty.SetClamped();

            pingDurationTimer.Start(
                () => // update  
                {
                    if (floaty)
                        floaty.SetAlpha(1 - pingDurationTimer.RelativeProgress);
                },
                () => // finish
                {
                    floaty.RequestDestroyFloaty();
                    Destroy(target);
                });
        }
        #endregion
    }
}