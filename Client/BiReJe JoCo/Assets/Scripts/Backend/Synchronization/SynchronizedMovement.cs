using UnityEngine;
using Photon.Pun;

namespace BiReJeJoCo.Backend
{
    public class SynchronizedMovement : TickBehaviour, IPunObservable
    {
        [SerializeField] MovementSynchType syncType;
        [SerializeField] float syncSpeed = 5;

        private PhotonView pv;

        private Vector3 targetPlayerPos = Vector3.zero;
        private Quaternion targetPlayerRot = Quaternion.identity;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
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
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
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
                LerpPosition(syncSpeed * globalVariables.GetVar<float>("move_sync_speed"), deltaTime);
            } 
            else if (syncType == MovementSynchType.MoveTowards)
            {
                MoveTowardsPosition(syncSpeed * globalVariables.GetVar<float>("move_sync_speed"), deltaTime);
            }
        }

        private void LerpPosition(float speed, float deltaTime)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, targetPlayerPos, deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPlayerRot, deltaTime * speed);
        }

        private void MoveTowardsPosition(float speed, float deltaTime)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.MoveTowards(transform.position, targetPlayerPos, deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPlayerRot, deltaTime * speed);
        }
    }
}