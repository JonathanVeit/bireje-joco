using UnityEngine;
using BiReJeJoCo.Backend;
using System;

namespace BiReJeJoCo.Map
{
    public class ElevatorPlattform : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] float moveSpeed = 8f;
        [SerializeField] float emptyMoveSpeedMultiplier = 1.5f;
        [SerializeField] float stopAtDistance = 0.1f;
        [SerializeField] CollisionTrigger trigger;

        [Space(10)]
        [SerializeField] Transform userGround;

        private Transform target;
        private Action onReachedTargetCallback;

        public bool ReachedTarget => target == null ? true : Vector3.Distance(transform.position, target.position) <= stopAtDistance;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            trigger.OnRemoteEntered += OnUserEntered;
            trigger.OnRemoteLeft += OnUserLeft;
            tickSystem.Register(this, "update");
        }
        #endregion

        public void SetTarget(Transform target, Action onReachedTarget = null)
        {
            this.target = target;
            onReachedTargetCallback = onReachedTarget;
        }

        public override void Tick(float deltaTime)
        {
            if (target == null) return;

            if (!ReachedTarget)
            {
                Move(deltaTime);
            }
            else if (onReachedTargetCallback != null)
            {
                onReachedTargetCallback?.Invoke();
                onReachedTargetCallback = null;
            }
        }

        private void Move(float deltaTime)
        {
            var direction = target.position - transform.position;
            var velocity = direction.normalized * moveSpeed * deltaTime;
            if (trigger.Remote.Count == 0 && trigger.Local == null)
            {
                velocity *= emptyMoveSpeedMultiplier;
            }

            transform.position += velocity;
            if (trigger.Local)
            {
                trigger.Local.transform.position += velocity;
            }
        }

        private void OnUserEntered(GameObject user)
        {
            user.GetComponentInParent<IPlayerObserved>().Owner.PlayerCharacter.syncedTransform.SetGround(userGround);
        }

        private void OnUserLeft(GameObject user)
        {
            user.GetComponentInParent<IPlayerObserved>().Owner.PlayerCharacter.syncedTransform.SetGround(null);
        }
    }
}
