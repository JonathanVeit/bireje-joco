using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace BiReJeJoCo.Backend
{
    [RequireComponent(typeof(PhotonView))]
    public class SyncVarDispatcher : SystemBehaviour, IPunObservable
    {
        [SerializeField] List<byte> syncedVariables;

        private Dictionary<byte, ISyncVar> observedVariables;
        private Dictionary<byte, string> variableCache;

        private PhotonView photonView;
        private Player player;

        private void Awake()
        {
            observedVariables = new Dictionary<byte, ISyncVar>();
            variableCache = new Dictionary<byte, string>();
            photonView = GetComponent<PhotonView>();

            player = playerManager.GetPlayer(photonView.Owner.ActorNumber);
            syncVarHub.RegisterDispatcher(player, this);
            this.gameObject.name = $"{player.NickName}'s Dispatcher";
            transform.SetParent(syncVarHub.transform);
        }

        public void RegisterSyncVar(ISyncVar syncVar)
        {
            if (observedVariables.ContainsKey(syncVar.UniqueId.Value))
            {
                Debug.LogError($"SyncVarDispatcher of player {player.NickName} already has a variable registed for id {syncVar.UniqueId}." +
                               $"Cannot sync multiple variables for the same id.");
                return;
            }

            observedVariables.Add(syncVar.UniqueId.Value, syncVar);
            syncedVariables.Add(syncVar.UniqueId.Value);
        }

        public bool TryUnregisterSyncVar(ISyncVar syncVar)
        {
            if (observedVariables.ContainsKey(syncVar.UniqueId.Value))
            {
                observedVariables.Remove(syncVar.UniqueId.Value);
                syncedVariables.Remove(syncVar.UniqueId.Value);
                return true;
            }

            return false;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                foreach (var curVariable in observedVariables.Values.ToArray())
                {
                    TrySentSyncVar(stream, curVariable);
                }
            }
            else
            {
                var streamValues = stream.ToArray();
                foreach (var curValue in streamValues)
                {
                    TryLoadSyncVar(curValue);
                }
            }
        }

        private void TrySentSyncVar(PhotonStream stream, ISyncVar syncVar)
        {
            if (!ValidateSyncVar(syncVar))
            {
                Debug.LogError($"Invalid SyncVar found on Dispatcher of player {player.NickName}");
                return;
            }

            // skip cached 
            var valueAsString = syncVar.GetSerializedString();
            if (!syncVar.IsForced && variableCache.ContainsKey(syncVar.UniqueId.Value) &&
                valueAsString == variableCache[syncVar.UniqueId.Value])
                return;

            // convert to serializable & send
            var value = syncVar.GetSerialized();
            var photonVar = new SerializableVariable(syncVar.UniqueId.Value, value);
            stream.SendNext(photonVar);

            // save cache
            if (!variableCache.ContainsKey(syncVar.UniqueId.Value))
                variableCache.Add(syncVar.UniqueId.Value, default);
            variableCache[syncVar.UniqueId.Value] = valueAsString;
        }

        private void TryLoadSyncVar(object rawSyncVar)
        {
            if (rawSyncVar is SerializableVariable serializableVar)
            {
                if (!observedVariables.ContainsKey(serializableVar.identifier))
                {
                    Debug.LogWarning($"No receiver found for SyncVar {serializableVar.identifier}", this.gameObject);
                    return;
                }

                if (!ValidateSyncVar(observedVariables[serializableVar.identifier]))
                {
                    Debug.LogError($"Invalid SyncVar found on Dispatcher of player {player.NickName}");
                    return;
                }

                observedVariables[serializableVar.identifier].SetSerialized(serializableVar.value);
            }
        }

        private bool ValidateSyncVar(ISyncVar syncVar)
        {
            return syncVar != null && syncVar.UniqueId.HasValue;
        }

        public struct SerializableVariable
        {
            public byte identifier;
            public byte[] value;

            public SerializableVariable(byte identifier, byte[] value)
            {
                this.identifier = identifier;
                this.value = value;
            }

            public static object Deserialize(byte[] data)
            {
                var result = new SerializableVariable();

                result.identifier = data[0];
                byte[] valuePart = GetValuePart(data);
                result.value = valuePart;

                return result;
            }

            private static byte[] GetValuePart(byte[] data)
            {
                var valuePart = new byte[data.Length - 1];
                for (int i = 1; i < data.Length; i++)
                    valuePart[i - 1] = data[i];
                return valuePart;
            }

            public static byte[] Serialize(object customType)
            {
                var c = (SerializableVariable)customType;
                var result = new byte[c.value.Length + 1];

                result[0] = c.identifier;
                for (int i = 0; i < c.value.Length; i++)
                    result[i + 1] = c.value[i];

                return result;
            }
        }
    }
}