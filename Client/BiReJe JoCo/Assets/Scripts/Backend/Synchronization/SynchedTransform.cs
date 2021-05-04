using Photon.Pun;
using System;

namespace BiReJeJoCo.Backend
{
    using JoVei.Base;
    using UnityEngine;

    [System.Flags]
    public enum SyncedTransformType
    {
        Position = 1 << 0,
        Rotation = 1 << 1,
    }

    public class SynchedTransform : SystemBehaviour, IPunObservable, IPlayerObserved
    {
        public event Action<Vector3> OnUpdatePosition;

        [SerializeField] SyncedTransformType type;
        [SerializeField] bool m_UseLocal;
        [SerializeField] float teleportAt;

        private float m_Distance;
        private float m_Angle;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;

        private Quaternion m_NetworkRotation;


        bool m_firstTake = false;
        PhotonView photonView;

        public void Initialize(PlayerControlled controller)
        {
            m_StoredPosition = transform.localPosition;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
            photonView = controller.PhotonView;
        }

        private void Reset()
        {
            // Only default to true with new instances. useLocal will remain false for old projects that are updating PUN.
            m_UseLocal = true;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (this.photonView.IsMine) return;

            if (type.HasFlag(SyncedTransformType.Position))
                UpdatePosition();
            if (type.HasFlag(SyncedTransformType.Rotation))
                UpdateRotation();
        }

        private void UpdatePosition()
        {
            bool teleport = IsTeleportRequired();

            if (m_UseLocal)
            {
                var tmp = transform.localPosition;
                if (teleport)
                    transform.localPosition = m_NetworkPosition;
                else
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                OnUpdatePosition?.Invoke(transform.localPosition - transform.position);
            }
            else
            {
                var tmp = transform.position;
                if (teleport)
                    transform.position = m_NetworkPosition;
                else
                    transform.position = Vector3.MoveTowards(transform.position, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                OnUpdatePosition?.Invoke(transform.position - transform.position);
            }
        }

        private bool IsTeleportRequired()
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            var tr = transform;

            // Write
            if (stream.IsWriting)
            {
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
            // Read
            else
            {
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
        }
    }
}