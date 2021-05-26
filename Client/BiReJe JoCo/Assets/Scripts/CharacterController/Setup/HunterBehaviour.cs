using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;
using JoVei.Base.UI;
using JoVei.Base.Helper;
using BiReJeJoCo.Items;

namespace BiReJeJoCo.Character
{
    public class HunterBehaviour : BaseBehaviour
    {
        [Header("Settings")]
        [SerializeField] Transform cameraRoot;
        [SerializeField] Transform fpsSetup;
        [SerializeField] Timer pingDurationTimer;
        [SerializeField] Timer pingCooldownTimer;   

        [Space(10)]
        [SerializeField] ShockGun gun;
        [SerializeField] float shootRange;
        [SerializeField] [Range(0, 360)] float autoAimAngle;
        [SerializeField] LayerMask shootTargetLayer;

        [Header("Runtime")]
        public SyncVar<bool> isHitting = new SyncVar<bool>(1, false);
        
        private SyncVar<Vector3?> shootPosition = new SyncVar<Vector3?>(2, null);
        private SyncVar<Vector3> pingPosition = new SyncVar<Vector3>(3);
        private GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private HunterPingFloaty pingFloaty;

        #region Initialization
        protected override void OnBehaviourInitialized()
        {
            if (Owner.IsLocalPlayer)
            {
                ConnectEvents();
                SetupPerspective();
            }
            else 
            {
                if (localPlayer.Role == PlayerRole.Hunter)
                    pingPosition.OnValueReceived += OnPingUpdated;
                shootPosition.OnValueReceived += (x) => gun.Shoot(x);
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
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnClosEMatch);

        
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
            pingPosition.OnValueReceived -= OnPingUpdated;
            if (syncVarHub)
            {
                syncVarHub.UnregisterSyncVar(shootPosition);
                syncVarHub.UnregisterSyncVar(pingPosition);
                syncVarHub.UnregisterSyncVar(isHitting); 
            }
        }
        #endregion

        #region Shoot
        private void OnShootHold(float duration)
        {
            shootPosition.SetValue(CalculateShootTarget());
            shootPosition.ForceSend();
            gun.Shoot(shootPosition.GetValue().Value);
        }
        private void OnShootReleased()
        {
            shootPosition.SetValue(null);
            gun.Shoot(null);
            isHitting.SetValue(false);
        }

        private Vector3 CalculateShootTarget()
        {
            var allHunted = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted);

            if (allHunted.Length > 0)
            {
                var huntedRoot =  allHunted[0].PlayerCharacter.ControllerSetup.ModelRoot;

                if (Vector3.Distance(gun.RayOrigin.position, huntedRoot.position) < shootRange)
                {
                    var dirToHunted = huntedRoot.position - gun.RayOrigin.position;
                    var gunDir = gun.RayOrigin.forward;
                    var angle = Vector3.Angle(dirToHunted, gunDir);

                    if (angle <= autoAimAngle)
                    {
                        isHitting.SetValue(true);
                        return huntedRoot.position;
                    }
                }
            }
            isHitting.SetValue(false);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootRange, shootTargetLayer, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            else
            {
                return Camera.main.transform.position + Camera.main.transform.forward * shootRange;
            }
        }
        #endregion

        #region Ping
        private void OnSpecial1Pressed()
        {
            if (pingCooldownTimer.State == TimerState.Counting) return;

            pingPosition.SetValue(transform.position + Vector3.up);
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
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootHold += OnShootHold;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootReleased += OnShootReleased;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed += OnSpecial1Pressed;
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