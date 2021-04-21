using UnityEngine;
using Photon.Pun;

namespace BiReJeJoCo.Backend
{
    public class SynchronizedMovement : TickBehaviour, IPunObservable
    {
        [SerializeField] MovementSynchType syncType;
        [SerializeField] Transform positionTarget;
        [SerializeField] Transform rotationTarget;
        [SerializeField] float positionSyncSpeed = 9;
        [SerializeField] float rotationSyncSpeed = 9;

        private PhotonView pv;

        private Vector3 targetPlayerPos = Vector3.zero;
        private Quaternion targetPlayerRot = Quaternion.identity;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            if (rotationTarget == null) rotationTarget = GetComponent<Transform>();
            if (positionTarget == null) positionTarget = GetComponent<Transform>();

            base.OnSystemsInitialized();
            pv = GetComponent<PhotonView>();

            if (!IsObserved()) 
            {
                throw new System.Exception(string.Format("PhotonView of {0} is beeing not observed.", this.gameObject));
            }
        }

        private bool IsObserved()
        {
            foreach (Component observedComponent in pv.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(positionTarget.position);
                stream.SendNext(rotationTarget.rotation);
            }
            else
            {
                //Network player, receive data
                targetPlayerPos = (Vector3)stream.ReceiveNext();
                targetPlayerRot = (Quaternion)stream.ReceiveNext();
            }
        }

        public override void Tick(float deltaTime)
        {
            if (pv.IsMine) return;

            if (syncType == MovementSynchType.Lerp)
            {
                LerpPosition(positionSyncSpeed * globalVariables.GetVar<float>("move_sync_speed"), deltaTime);
            } 
            else if (syncType == MovementSynchType.MoveTowards)
            {
                MoveTowardsPosition(rotationSyncSpeed * globalVariables.GetVar<float>("rot_sync_speed"), deltaTime);
            }
        }

        private void LerpPosition(float speed, float deltaTime)
        {
            positionTarget.position = Vector3.Lerp(positionTarget.position, targetPlayerPos, deltaTime * speed);
            rotationTarget.rotation = Quaternion.Lerp(rotationTarget.rotation, targetPlayerRot, deltaTime * speed);
        }

        private void MoveTowardsPosition(float speed, float deltaTime)
        {
            positionTarget.position = Vector3.MoveTowards(positionTarget.position, targetPlayerPos, deltaTime * speed);
            rotationTarget.rotation = Quaternion.RotateTowards(rotationTarget.rotation, targetPlayerRot, deltaTime * speed);
        }
    }
}