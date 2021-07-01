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

    public class SynchedTransform : TickBehaviour, IPunObservable, IPlayerObserved, IMovementVelocitySource
    {
        [Header("Settings")]
        [SerializeField] SyncedTransformType type;
        [SerializeField] bool m_UseLocal;
        [SerializeField] float teleportAt;
        [SerializeField] Transform movementTarget;
        [SerializeField] Transform rotationTarget;
        [SerializeField] [Range(0.1f, 2)]float smoothSyncSpeed = 1f;

        [Space(10)]
        [SerializeField] float snapGroundTreshold = 0.2f;

        [Header("Debugging")]
        [SerializeField] float m_Distance;
        [SerializeField] float m_Angle;

        [SerializeField] Vector3 m_Direction;
        [SerializeField] Vector3 m_NetworkPosition;
        [SerializeField] Vector3 m_StoredPosition;
        [SerializeField] Vector3 m_NetworkVelocity;

        [SerializeField] Quaternion m_NetworkRotation;
        [SerializeField] Transform ground;

        bool m_firstTake = false;
        IMovementVelocitySource velocitySource;

        private PlayerControlled controller;
        public Player Owner => controller.Player;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            m_StoredPosition = movementTarget.localPosition;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;

            if (!movementTarget)
                movementTarget = transform;
            if (!rotationTarget)
                rotationTarget = transform;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }
        #endregion

        #region Update Transform
        public override void Tick(float deltaTime)
        {
            if (controller.PhotonView.IsMine || ! this.enabled) return;

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
                    movementTarget.localPosition = m_NetworkPosition;
                    return;
                }
                    
                var targetPosition = Vector3.MoveTowards(movementTarget.localPosition, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate) * smoothSyncSpeed);
                targetPosition = SnapToGround(targetPosition);
                movementTarget.localPosition = targetPosition;
            }
            else
            {
                if (teleport)
                {
                    movementTarget.position = m_NetworkPosition;
                    return;
                }

                var targetPosition = Vector3.MoveTowards(movementTarget.position, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate) * smoothSyncSpeed);
                targetPosition = SnapToGround(targetPosition);
                movementTarget.position = targetPosition;
            }
        }

        private void UpdateRotation()
        {
            if (m_UseLocal)
            {
                rotationTarget.localRotation = Quaternion.RotateTowards(rotationTarget.localRotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate) * smoothSyncSpeed);
            }
            else
            {
                rotationTarget.rotation = Quaternion.RotateTowards(rotationTarget.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate) * smoothSyncSpeed);
            }
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
            if (type.HasFlag(SyncedTransformType.Position))
            {
                if (m_UseLocal)
                {
                    this.m_Direction = movementTarget.localPosition - this.m_StoredPosition;
                    this.m_StoredPosition = movementTarget.localPosition;
                    stream.SendNext(movementTarget.localPosition);
                    stream.SendNext(this.m_Direction);
                }
                else
                {
                    this.m_Direction = movementTarget.position - this.m_StoredPosition;
                    this.m_StoredPosition = movementTarget.position;
                    stream.SendNext(movementTarget.position);
                    stream.SendNext(this.m_Direction);
                }

                if (velocitySource != null)
                {
                    this.m_NetworkVelocity = velocitySource.GetMovementVelocity();
                    stream.SendNext(m_NetworkVelocity);
                }
            }

            if (type.HasFlag(SyncedTransformType.Rotation))
            {
                if (m_UseLocal)
                {
                    stream.SendNext(rotationTarget.localRotation);
                }
                else
                {
                    stream.SendNext(rotationTarget.rotation);
                }
            }
        }

        private void ReadStream(PhotonStream stream, PhotonMessageInfo info)
        {
            if (type.HasFlag(SyncedTransformType.Position))
            {
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_Direction = (Vector3)stream.ReceiveNext();
                if (m_firstTake)
                {
                    if (m_UseLocal)
                        movementTarget.localPosition = this.m_NetworkPosition;
                    else
                        movementTarget.position = this.m_NetworkPosition;

                    this.m_Distance = 0f;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    this.m_NetworkPosition += this.m_Direction * lag;
                    if (m_UseLocal)
                    {
                        this.m_Distance = Vector3.Distance(movementTarget.localPosition, this.m_NetworkPosition);
                    }
                    else
                    {
                        this.m_Distance = Vector3.Distance(movementTarget.position, this.m_NetworkPosition);
                    }
                }

            }

            if (stream.PeekNext() is Vector3)
            {
                this.m_NetworkVelocity = (Vector3) stream.ReceiveNext();
            }

            if (type.HasFlag(SyncedTransformType.Rotation))
            {
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (m_firstTake)
                {
                    this.m_Angle = 0f;

                    if (m_UseLocal)
                    {
                        rotationTarget.localRotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        rotationTarget.rotation = this.m_NetworkRotation;
                    }
                }
                else
                {
                    if (m_UseLocal)
                    {
                        this.m_Angle = Quaternion.Angle(rotationTarget.localRotation, this.m_NetworkRotation);
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(rotationTarget.rotation, this.m_NetworkRotation);
                    }
                }
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
        #endregion

        #region Public Member
        public void SetMovementTarget(Transform target)
        {
            movementTarget = target;
        }
        public void SetRotationTarget(Transform target)
        {
            rotationTarget = target;
        }
        public void SetVelocitySource(IMovementVelocitySource source)
        {
            velocitySource = source;
        }

        public void SetGround(Transform ground)
        {
            this.ground = ground;
        }

        public Vector3 GetMovementVelocity() { return m_NetworkVelocity; }
        #endregion

        #region Helper
        private bool TeleportRequired()
        {
            if (m_UseLocal)
            {
                return Vector3.Distance(movementTarget.localPosition, m_NetworkPosition) > teleportAt;
            }
            else
            {
                return Vector3.Distance(movementTarget.position, m_NetworkPosition) > teleportAt;
            }
        }

        private Vector3 SnapToGround(Vector3 position)
        {
            if (!ground)
                return position;

            if (Mathf.Abs(m_NetworkVelocity.y) <= snapGroundTreshold || 
                position.y < ground.position.y)
            {
                position.y = ground.position.y;
            }

            return position;
        }
        #endregion
    }
}