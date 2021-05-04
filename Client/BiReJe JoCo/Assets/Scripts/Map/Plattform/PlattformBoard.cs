using BiReJeJoCo.Backend;
using Photon.Pun;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class PlattformBoard : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] float moveSpeed = 8f;
        [SerializeField] float stopAtDistance = 0.1f;

        private PlattformBoardTrigger trigger;
        private PlayerControlled controller;

        private Transform currentTarget;

        public bool ReachedTarget => currentTarget == null ? true : Vector3.Distance(transform.position, currentTarget.position) <= stopAtDistance;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            trigger = GetComponentInChildren<PlattformBoardTrigger>();
        }

        public void Initialize(byte plattformId)
        {
            controller.PhotonView.RPC("PlattformBoard_Initialize", RpcTarget.All, plattformId);
        }

        [PunRPC]
        private void PlattformBoard_Initialize(byte plattformId) 
        {
            var plattform = (SynchronizedTrigger.Find(plattformId) as TriggeredPlattform);
            plattform.RegisterBoard(this);
            transform.SetParent(plattform.transform);
        }
        #endregion

        public void SetTarget(Transform target)
        {
            currentTarget = target;
        }

        public void Update()
        {
            if (localPlayer.IsHost)
            {
                if (currentTarget != null && !ReachedTarget)
                {
                    var direction = currentTarget.position - transform.position;

                    var velocity = direction.normalized * moveSpeed * Time.deltaTime;
                    transform.position += velocity;
                    UpdateUser(velocity);
                }
            }
        }

        private void UpdateUser(Vector3 velocity)
        {
            foreach (var curUser in trigger.User)
            {
                //curUser.AddMomentum(velocity * 1f);
            }
        }
    }
}
