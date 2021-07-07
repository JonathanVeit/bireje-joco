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
        [SerializeField] Vector3 autoAimOffset;
        [SerializeField] float coralDestroyRadius;
        [SerializeField] LayerMask coralLayer;
        [SerializeField] SkinnedMeshRenderer ammoRenderer;
        [SerializeField] int materialIndex;

        public ShockGun Gun => gun;
        public bool IsHittingHunted => isHittingHunted.GetValue();
        public bool IsShooting { get; private set; }
        public bool IsReloading => reloadTimer.State == TimerState.Counting;


        private SyncVar<bool> isHittingHunted = new SyncVar<bool>(1, false);
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
                if (isHittingHunted.GetValue() && x.HasValue)
                    gun.Shoot(huntedTransform.position + autoAimOffset);
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
                syncVarHub.UnregisterSyncVar(isHittingHunted);
                syncVarHub.UnregisterSyncVar(shootPosition);
            }

            messageHub.UnregisterReceiver(this);
            reloadTimer.Stop();
        }
        #endregion

        public void Shoot()
        {
            if (reloadTimer.State != TimerState.Finished || 
                Behaviour.TrapMechanic.IsThrowingTrap) return;

            var shootTarget = CalculateShootTarget();
            shootPosition.SetValue(shootTarget);
            gun.Shoot(shootPosition.GetValue().Value);

            ammoCounter.CountUp(() =>
            {
                Reload();
            });
            UpdateAmmoAmount(1 - ammoCounter.RelativeProgress);

            if (!IsShooting)
            {
                Owner.PlayerCharacter.ControllerSetup.AnimationController.SetTrigger("start_shoot");
                Owner.PlayerCharacter.ControllerSetup.AnimationController.BlockParameters("jump", "fall");
                IsShooting = true;
            }
        }
        public void StopShooting(bool callAnimationTrigger = true)
        {
            shootPosition.SetValue(null);
            gun.Shoot(null);
            isHittingHunted.SetValue(false);

            if (IsShooting)
            {
                if (callAnimationTrigger &&
                    reloadTimer.State != TimerState.Counting)
                {
                    Owner.PlayerCharacter.ControllerSetup.AnimationController.SetTrigger("end_shoot");
                    Owner.PlayerCharacter.ControllerSetup.AnimationController.UnblockParameters("jump", "fall");
                }
                IsShooting = false;
            }
        }
        public void Reload ()
        {
            if (ammoCounter.RelativeProgress == 0 ||
                reloadTimer.State == TimerState.Counting || 
                Behaviour.TrapMechanic.IsThrowingTrap) return;

            Owner.PlayerCharacter.ControllerSetup.AnimationController.SetTrigger("reload");
            Owner.PlayerCharacter.ControllerSetup.AnimationController.BlockParameters("jump", "land", "fall", "start_shoot", "throw_trap");
            StopShooting(false);

            reloadTimer.Start(
                () =>
                {
                    UpdateAmmoAmount(reloadTimer.RelativeProgress);

                }, // update 
                () => 
                {
                    ammoCounter.SetValue(0);
                    Owner.PlayerCharacter.ControllerSetup.AnimationController.UnblockParameters("jump", "land", "fall", "start_shoot", "throw_trap");
                }); // finish
        }

        private Vector3 CalculateShootTarget()
        {
            var ray = new Ray()
            {
                origin = Camera.main.transform.position,
                direction = Camera.main.transform.forward,
            };

            if (huntedTransform == null)
                return CastToTarget(ray, out var b);

            if (Vector3.Distance(gun.RayOrigin.position, huntedTransform.position + autoAimOffset) < range && 
                !HuntedIsTranformed())
            {
                var dirToHunted = huntedTransform.position + autoAimOffset - gun.RayOrigin.position;
                var gunDir = gun.RayOrigin.forward;
                var angle = Vector3.Angle(dirToHunted, gunDir);

                if (angle <= autoAimAngle)
                {
                    ray.origin = gun.RayOrigin.position;
                    ray.direction = dirToHunted;
                }
            }

            var hitPoint =  CastToTarget(ray, out bool isHittingHunted);
            this.isHittingHunted.SetValue(isHittingHunted);
            if (isHittingHunted)
            {
                DestroyCorals(hitPoint);
                return hitPoint;
            }

            ray.direction = gun.RayOrigin.forward;
            hitPoint = CastToTarget(ray, out isHittingHunted);
            DestroyCorals(hitPoint);
            return hitPoint;
        }
        private Vector3 CastToTarget(Ray ray, out bool isHittingHunted)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range, targetLayer, QueryTriggerInteraction.Ignore))
            {
                isHittingHunted = hit.collider.gameObject.layer == 10;            
                return hit.point;
            }
            
            isHittingHunted = false;
            return Camera.main.transform.position + Camera.main.transform.forward * range;
        }

        private void DestroyCorals(Vector3 point) 
        {
            var collider = Physics.OverlapSphere(point, coralDestroyRadius, coralLayer);

            foreach (var col in collider)
                col.gameObject.GetComponent<DestroyableCoral>().Destroy();
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
        private bool HuntedIsTranformed() 
        {
            var allHunted = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted);
            if (allHunted.Length == 0)
                return false;

            return allHunted[0].PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().TransformationMechanic.IsTransformed;
        }

        private void UpdateAmmoAmount(float value) 
        {
            Debug.Log(ammoRenderer.materials[materialIndex].GetFloat("Amount"));
            ammoRenderer.materials[materialIndex].SetFloat("Amount", value);
        }
        #endregion
    }
}