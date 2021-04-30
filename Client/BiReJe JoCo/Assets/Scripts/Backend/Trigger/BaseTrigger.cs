using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using System.Collections;

namespace BiReJeJoCo.Backend
{
    public abstract class BaseTrigger : TickBehaviour
    {
        [Header ("Base Trigger Settings")]
        [SerializeField] protected TriggerTarget target;
        [SerializeField] protected float range = 8;
        [SerializeField] protected float coolDown = 1f;
        [SerializeField] protected string floatingElementId = "default_trigger";
        [SerializeField] protected Transform floatingElementTarget;
        [SerializeField] protected Vector2 floatingElementOffset;
        [SerializeField] protected MeshRenderer floatingElementRenderer;

        [Header("Debugging")]
        [SerializeField] protected Transform playerTransform;
        [SerializeField] protected FloatingElement floaty;
        [SerializeField] protected bool isCoolDown = false;

        #region Initialization
        protected sealed override void OnSystemsInitialized()
        {
            if (PlayerRoleMatchesTarget(localPlayer.Role))
            {
                tickSystem.Register(this, "update_half_second");
                messageHub.RegisterReceiver<OnPlayerPressedTriggerMsg>(this, OnPressedTrigger);

                if (floatingElementTarget == null)
                    floatingElementTarget = transform;

                OnSetupActive();
            }
        }

        protected virtual void OnSetupActive() { }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            messageHub.UnregisterReceiver(this);

            if (floaty != null)
            {
                floatingManager.DestroyElement(floaty);
            }
        }
        #endregion

        public override void Tick(float deltaTime)
        {
            if (PlayerIsInRange())
            {
                if (floaty == null)
                {
                    var config = new FloatingElementConfig(floatingElementId, gameUI.floatingElementGrid, floatingElementTarget, floatingElementOffset);
                    floaty = floatingManager.GetElementAs<FloatingElement>(config);
                    floaty.SetVisibleRenderer(floatingElementRenderer);
                }
            }
            else
            {
                if (floaty != null)
                {
                    floatingManager.DestroyElement(floaty);
                    floaty = null;
                }
            }
        }

        protected virtual void OnPressedTrigger(OnPlayerPressedTriggerMsg msg)
        {
            if (PlayerIsInRange() && ! isCoolDown)
            {
                OnTriggerInteracted();
                StartCoroutine(CoolDown());
            }
        }
        protected abstract void OnTriggerInteracted();

        #region Helper
        protected bool PlayerRoleMatchesTarget(PlayerRole role)
        {
            if (target == TriggerTarget.AllPlayer) return true;

            switch (role)
            {
                case PlayerRole.Hunted:
                    return target == TriggerTarget.Hunted;
                case PlayerRole.Hunter:
                    return target == TriggerTarget.Hunter;

                default:
                    return false;
            }
        }

        protected bool PlayerIsInRange() 
        {
            if (playerTransform == null)
            {
                if (localPlayer.PlayerCharacter)
                    playerTransform = localPlayer.PlayerCharacter.GetComponentInChildren<Mover>().transform;
                return false;
            }
            else
                return Vector3.Distance(playerTransform.position, transform.position) <= range;
        }

        protected IEnumerator CoolDown()
        {
            isCoolDown = true;
            yield return new WaitForSeconds(coolDown);
            isCoolDown = false;
        }
        #endregion
    }
}
