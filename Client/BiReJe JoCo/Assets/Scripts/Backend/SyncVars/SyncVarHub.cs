using JoVei.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ExitGames.Client.Photon;

namespace BiReJeJoCo.Backend
{
    public class SyncVarHub : TickBehaviour
    {
        private Dictionary<Player, List<ISyncVar>> variablesToLoad;
        private Dictionary<Player, SyncVarDispatcher> loadeDispatcher;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DontDestroyOnLoad(this);
            DIContainer.RegisterImplementation<SyncVarHub>(this);
            PhotonPeer.RegisterType(typeof(SyncVarDispatcher.SerializableVariable), 100, SyncVarDispatcher.SerializableVariable.Serialize, SyncVarDispatcher.SerializableVariable.Deserialize);
            
            variablesToLoad = new Dictionary<Player, List<ISyncVar>>();
            loadeDispatcher = new Dictionary<Player, SyncVarDispatcher>();
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DIContainer.UnregisterImplementation<SyncVarHub>();
        }
        #endregion

        public void RegisterDispatcher(Player owner, SyncVarDispatcher dispatcher) 
        {
            if (loadeDispatcher.ContainsKey(owner)) 
            {
                Debug.LogError($"There is already a SyncVarDispatcher registered for owner {owner.NickName}.");
                return;
            }

            this.loadeDispatcher.Add(owner, dispatcher);
        }

        public void UnregisterDispatcher(SyncVarDispatcher dispatcher)
        {
            foreach (var curEntry in loadeDispatcher.ToArray())
            {
                if (curEntry.Value == dispatcher)
                {
                    loadeDispatcher.Remove(curEntry.Key);
                    return;
                }
            }
        }

        public void RegisterSyncVar(Player owner, ISyncVar syncVar) 
        {
            if (!syncVar.UniqueId.HasValue)
            {
                Debug.LogError($"Cannot registerd not initalizes SyncVar for player {owner.NickName}.");
                return;
            }

            if (loadeDispatcher.ContainsKey(owner))
            {
                RegisterSyncVarInternal(owner, syncVar);
                return;
            }

            if (owner.IsLocalPlayer&& !DispatcherIsRequested(owner))
            {
                photonRoomWrapper.Instantiate("sync_var_dispatcher", Vector3.zero, Quaternion.identity);
            }

            if (!variablesToLoad.ContainsKey(owner))
                variablesToLoad.Add(owner, new List<ISyncVar>());
            variablesToLoad[owner].Add(syncVar);
        }

        public void UnregisterSyncVar(ISyncVar syncVar)
        {
            foreach (var curDispatcher in loadeDispatcher.Values)
            {
                if (curDispatcher.TryUnregisterSyncVar(syncVar))
                    return;
            }
        }


        private bool DispatcherIsRequested(Player forPlayer) 
        {
            return variablesToLoad.ContainsKey(forPlayer);
        }

        public override void Tick(float deltaTime)
        {
            foreach (var curEntry in variablesToLoad.ToArray())
            {
                if (!loadeDispatcher.ContainsKey(curEntry.Key))
                    continue;
                
                foreach (var curSyncVar in curEntry.Value.ToArray())
                {
                    RegisterSyncVarInternal(curEntry.Key, curSyncVar);
                }
            }
        }

        private void RegisterSyncVarInternal(Player owner, ISyncVar syncVar) 
        {
            loadeDispatcher[owner].RegisterSyncVar(syncVar);
            syncVar.Connect(owner.IsLocalPlayer ? SyncVarStatus.IsSending : SyncVarStatus.IsReceiving);
            variablesToLoad[owner].Remove(syncVar);
        }
    }
}