using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class ShockMechanic : BaseBehaviourMechanic<HunterBehaviour>
    {
        [Header("Settings")]
        [SerializeField] ShockGun gun;
        [SerializeField] Counter ammoCounter;
        [SerializeField] Timer reloadTimer;
        [SerializeField] float range;
        [SerializeField] [Range(0, 360)] float autoAimAngle;
        [SerializeField] LayerMask targetLayer;

        public SyncVar<bool> isHitting = new SyncVar<bool>(1, false);
        private SyncVar<Vector3?> shootPosition = new SyncVar<Vector3?>(2, null);
        private Transform huntedTransform => GetHuntedRoot();

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
            shootPosition.OnValueReceived += (x) =>
            {
                if (isHitting.GetValue() && x.HasValue)
                    gun.Shoot(huntedTransform.position);
                else
                    gun.Shoot(x);
            };
            
            ConnectEvents();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
        }
        private void DisconnectEvents()
        {
            if (syncVarHub)
            {
                syncVarHub.UnregisterSyncVar(isHitting);
                syncVarHub.UnregisterSyncVar(shootPosition);
            }
            
            messageHub.UnregisterReceiver(this);
            reloadTimer.Stop();
        }
        #endregion

        public void Shoot()
        {
            if (reloadTimer.State != TimerState.Finished) return;

            shootPosition.SetValue(CalculateShootTarget());
            gun.Shoot(shootPosition.GetValue().Value);

            ammoCounter.CountUp(() =>
            {
                StopShooting();
                reloadTimer.Start(() => gameUI.UpdateAmmoBar(reloadTimer.RelativeProgress), null);
            });
            gameUI.UpdateAmmoBar(1 - ammoCounter.RelativeProgress);
        }
        public void StopShooting()
        {
            shootPosition.SetValue(null);
            gun.Shoot(null);
            isHitting.SetValue(false);
        }

        private Vector3 CalculateShootTarget()
        {
            var ray = new Ray()
            {
                origin = Camera.main.transform.position,
                direction = Camera.main.transform.forward,
            };

            if (huntedTransform == null)
                return CastToTarget(ray);

            if (Vector3.Distance(gun.RayOrigin.position, huntedTransform.position) < range)
            {
                var dirToHunted = huntedTransform.position - gun.RayOrigin.position;
                var gunDir = gun.RayOrigin.forward;
                var angle = Vector3.Angle(dirToHunted, gunDir);

                if (angle <= autoAimAngle)
                {
                    ray.origin = gun.RayOrigin.position;
                    ray.direction = dirToHunted;
                }
            }

            return CastToTarget(ray);
        }
        private Vector3 CastToTarget(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range, targetLayer, QueryTriggerInteraction.Ignore))
            {
                
                isHitting.SetValue(hit.collider.gameObject.layer == 10);
                return hit.point;
            }
            
            isHitting.SetValue(false);
            return Camera.main.transform.position + Camera.main.transform.forward * range;
        }

        #region Helper
        private Transform GetHuntedRoot()
        {
            var allHunted = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted);
            if (allHunted.Length == 0)
                return null;

            var hunted = allHunted[0];
            var mechanic = hunted.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic;
            if (mechanic.IsTransformed && 
                mechanic.TransformedItem != null)
            {
                return mechanic.TransformedItem.transform;
            }

            return allHunted[0].PlayerCharacter.ControllerSetup.ModelRoot;
        }
        #endregion
    }
}