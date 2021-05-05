using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Map
{
    public class PlattformBoard : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] float moveSpeed = 8f;
        [SerializeField] float stopAtDistance = 0.1f;
        [SerializeField] CollisionTrigger trigger;

        [Space(10)]
        [SerializeField] Transform userGround;

        private Transform target;
        public bool ReachedTarget => target == null ? true : Vector3.Distance(transform.position, target.position) <= stopAtDistance;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            trigger.OnRemoteEntered += OnUserEntered;
            trigger.OnRemoteLeft += OnUserLeft;
            tickSystem.Register(this, "update");
        }
        #endregion

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public override void Tick(float deltaTime)
        {
            if (target != null && !ReachedTarget)
            {
                Move(deltaTime);
            }
        }

        private void Move(float deltaTime)
        {
            var direction = target.position - transform.position;
            var velocity = direction.normalized * moveSpeed * deltaTime;
            transform.position += velocity;

            if (trigger.Local)
            {
                trigger.Local.transform.position += velocity;
            }
        }

        private void OnUserEntered(GameObject user)
        {
            user.GetComponent<SynchedTransform>().SetGround(userGround);
        }

        private void OnUserLeft(GameObject user)
        {
            user.GetComponent<SynchedTransform>().SetGround(null);
        }
    }
}
