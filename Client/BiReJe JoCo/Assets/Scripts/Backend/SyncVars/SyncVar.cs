using Newtonsoft.Json;
using UnityEngine;
using ExitGames.Client.Photon;
using System;

namespace BiReJeJoCo.Backend
{
    public enum SyncVarStatus
    {
        NotConnected = 0,
        IsReceiving = 1,
        IsSending = 2,
    }

    public interface ISyncVar
    {
        SyncVarStatus Status { get; }
        byte? UniqueId { get; }
        void Connect(SyncVarStatus type);

        byte[] GetSerialized();
        string GetSerializedString();
        void SetSerialized(byte[] value);
    }

    [Serializable]
    public class SyncVar<TValue> : SystemAccessor, ISyncVar
    {
        public event Action<byte> OnValueReceived;

        public SyncVarStatus Status { get; private set; }
        public byte? UniqueId { get; private set; }
        [SerializeField] private TValue value;

        public SyncVar(byte uniqueId) : this(uniqueId, default) { }
        public SyncVar(byte uniqueId, TValue initialValue)
        {
            this.UniqueId = uniqueId;
            SetValue(initialValue);
        }

        public void SetValue(TValue value)
        {
            if (Status == SyncVarStatus.IsReceiving)
            {
                Debug.LogWarning($"SyncVar {UniqueId} cannot be set. Its Receiving.");
                return;
            }

            this.value = value;
        }
        public TValue GetValue()
        {
            return value;
        }


        public void Connect(SyncVarStatus type)
        {
            Status = type;
        }


        public void SetSerialized(byte[] value)
        {
            this.value = (TValue) Protocol.Deserialize(value);
            OnValueReceived.Invoke(UniqueId.Value);
    }
        public byte[] GetSerialized()
        {
            return Protocol.Serialize(value);
        }
        public string GetSerializedString() 
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
