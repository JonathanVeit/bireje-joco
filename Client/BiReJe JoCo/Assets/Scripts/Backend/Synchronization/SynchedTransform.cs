using Photon.Pun;
using UnityEngine;

namespace BiReJeJoCo.Backend
{
    [System.Flags]
    public enum SyncedTransformType
    {
        Position = 1 << 0,
        Rotation = 1 << 1,
    }

    public class SynchedTransform : TickBehaviour, IPunObservable, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] SyncedTransformType type;
        [SerializeField] bool m_UseLocal;
        [SerializeField] float teleportAt;
        [SerializeField] Rigidbody rigidBody;

        [Space(10)]
        [SerializeField] float snapGroundTreshold = 0.2f;

        [Header("Debugging")]
        [SerializeField] float m_Distance;
        [SerializeField] float m_Angle;

        [SerializeField] Vector3 m_Direction;
        [SerializeField] Vector3 m_NetworkPosition;
        [SerializeField] Vector3 m_StoredPosition;
        [SerializeField] float m_NetworkVelocity;

        [SerializeField] Quaternion m_NetworkRotation;
        [SerializeField] Transform overrideGround;

        bool m_firstTake = false;
        PhotonView photonView;

        private PlayerControlled controller;
        public Player Owner => controller.Player;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            m_StoredPosition = transform.localPosition;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
            photonView = controller.PhotonView;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }
        #endregion

        #region Update Transform
        public override void Tick(float deltaTime)
        {
            if (this.photonView.IsMine|| !this.enabled) return;

            if (type.HasFlag(SyncedTransformType.Position))
                UpdatePosition();
            if (type.HasFlag(SyncedTransformType.Rotation))
                UpdateRotation();
        }

        private void UpdatePosition()
        {
            bool teleport = TeleportRequired();

            if (m_UseLocal)
            {
                if (teleport)
                {
                    transform.localPosition = m_NetworkPosition;
                    return;
                }
                    
                var targetPosition = Vector3.MoveTowards(transform.localPosition, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                targetPosition = SnapToGround(targetPosition);
                transform.localPosition = targetPosition;
            }
            else
            {
                if (teleport)
                {
                    transform.position = m_NetworkPosition;
                    return;
                }

                var targetPosition = Vector3.MoveTowards(transform.position, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                targetPosition = SnapToGround(targetPosition);
                transform.position = targetPosition;
            }
        }

        private void UpdateRotation()
        {
            if (m_UseLocal)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void SetGround(Transform ground)
        {
            overrideGround = ground;
        }
        #endregion

        #region Synchronize 
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!this.enabled) return;

            // Write
            if (stream.IsWriting)
            {
                WriteStream(stream);
            }
            // Read
            else
            {
                ReadStream(stream, info);
            }
        }

        private void WriteStream(PhotonStream stream)
        {
            var tr = transform;

            if (type.HasFlag(SyncedTransformType.Position))
            {
                if (m_UseLocal)
                {
                    this.m_Direction = tr.localPosition - this.m_StoredPosition;
                    this.m_StoredPosition = tr.localPosition;
                    stream.SendNext(tr.localPosition);
                    stream.SendNext(this.m_Direction);
                }
                else
                {
                    this.m_Direction = tr.position - this.m_StoredPosition;
                    this.m_StoredPosition = tr.position;
                    stream.SendNext(tr.position);
                    stream.SendNext(this.m_Direction);
                }

                if (rigidBody)
                {
                    this.m_NetworkVelocity = rigidBody.velocity.y;
                    stream.SendNext(m_NetworkVelocity);
                }
            }

            if (type.HasFlag(SyncedTransformType.Rotation))
            {
                if (m_UseLocal)
                {
                    stream.SendNext(tr.localRotation);
                }
                else
                {
                    stream.SendNext(tr.rotation);
                }
            }
        }

        private void ReadStream(PhotonStream stream, PhotonMessageInfo info)
        {
            var tr = transform;

            if (type.HasFlag(SyncedTransformType.Position))
            {
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_Direction = (Vector3)stream.ReceiveNext();
                if (m_firstTake)
                {
                    if (m_UseLocal)
                        tr.localPosition = this.m_NetworkPosition;
                    else
                        tr.position = this.m_NetworkPosition;

                    this.m_Distance = 0f;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    this.m_NetworkPosition += this.m_Direction * lag;
                    if (m_UseLocal)
                    {
                        this.m_Distance = Vector3.Distance(tr.localPosition, this.m_NetworkPosition);
                    }
                    else
                    {
                        this.m_Distance = Vector3.Distance(tr.position, this.m_NetworkPosition);
                    }
                }

            }

            if (rigidBody)
            {
                this.m_NetworkVelocity = (float) stream.ReceiveNext();
            }

            if (type.HasFlag(SyncedTransformType.Rotation))
            {
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (m_firstTake)
                {
                    this.m_Angle = 0f;

                    if (m_UseLocal)
                    {
                        tr.localRotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        tr.rotation = this.m_NetworkRotation;
                    }
                }
                else
                {
                    if (m_UseLocal)
                    {
                        this.m_Angle = Quaternion.Angle(tr.localRotation, this.m_NetworkRotation);
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(tr.rotation, this.m_NetworkRotation);
                    }
                }
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
        #endregion

        #region Helper
        private bool TeleportRequired()
        {
            if (m_UseLocal)
            {
                return Vector3.Distance(transform.localPosition, m_NetworkPosition) > teleportAt;
            }
            else
            {
                return Vector3.Distance(transform.position, m_NetworkPosition) > teleportAt;
            }
        }

        private Vector3 SnapToGround(Vector3 position)
        {
            if (!overrideGround)
                return position;

            if (Mathf.Abs(m_NetworkVelocity) <= snapGroundTreshold || 
                position.y < overrideGround.position.y)
            {
                position.y = overrideGround.position.y;
            }

            return position;
        }
        #endregion
    }
}