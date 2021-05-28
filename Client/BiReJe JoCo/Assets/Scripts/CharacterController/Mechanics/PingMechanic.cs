using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using JoVei.Base.UI;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class PingMechanic : BaseBehaviourMechanic<HunterBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Timer pingDurationTimer;
        [SerializeField] Timer pingCooldownTimer;

        private SyncVar<Vector3> pingPosition = new SyncVar<Vector3>(3);
        private HunterPingFloaty pingFloaty;

        private GameUI gameUI => uiManager.GetInstanceOf<GameUI>();

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
            if (localPlayer.Role == PlayerRole.Hunter)
                pingPosition.OnValueReceived += OnPingUpdated;
            
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnCloseMatch);
        }
        private void DisconnectEvents()
        {
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(pingPosition);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);

            pingPosition.OnValueReceived -= OnPingUpdated;
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public void SpawnPing()
        {
            if (pingCooldownTimer.State == TimerState.Counting) return;

            pingPosition.SetValue(transform.position);
            pingPosition.ForceSend();
            pingCooldownTimer.Start(
                () => // update
                {
                    gameUI.UpdatePingCooldown(pingCooldownTimer.RelativeProgress);
                }, null);
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

        private void OnCloseMatch(PhotonMessage obj)
        {
            pingDurationTimer.Stop();
            pingCooldownTimer.Stop();

            if (pingFloaty)
                pingFloaty.RequestDestroyFloaty();
        }
    }
}