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
        [SerializeField] Transform pingSpawnPoint;
        [SerializeField] float throwStrength;
        [SerializeField] Vector3 additionalThrowDirection;
        [SerializeField] Vector3 throwTorque;

        private SyncVar<Vector3> pingPosition = new SyncVar<Vector3>(3, true);
        private HunterPingFloaty pingFloaty;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            gameUI.UpdatePingCooldown(1);
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
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
            OnPingUpdated(pingPosition.GetValue());
            pingPosition.ForceSend();
            pingCooldownTimer.Start(
                () => // update
                {
                    gameUI.UpdatePingCooldown(pingCooldownTimer.RelativeProgress);
                }, null);
        }
        private void OnPingUpdated(Vector3 position)
        {
            if (pingDurationTimer.State == TimerState.Counting)
                pingDurationTimer.Stop(true);

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunter_ping_marker");
            var target = Instantiate(prefab, pingSpawnPoint.position, pingSpawnPoint.rotation);
            target.GetComponent<Rigidbody>().AddForce((pingSpawnPoint.forward + additionalThrowDirection) * throwStrength, ForceMode.Impulse);
            target.GetComponent<Rigidbody>().AddTorque(throwTorque);
            soundEffectManager.Play("hunter_ping", pingSpawnPoint);

            if (!Owner.IsLocalPlayer && 
                localPlayer.Role == PlayerRole.Hunter)
            {
                var parent = uiManager.GetInstanceOf<GameUI>().floatingElementGrid;
                target.transform.position = position;

                var config = new FloatingElementConfig("hunter_ping", parent, target.transform);
                pingFloaty = floatingManager.GetElementAs<HunterPingFloaty>(config);
                pingFloaty.SetClamped();
            }

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
                    }

                    if (target)
                        Destroy(target);
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