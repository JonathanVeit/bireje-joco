using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace BiReJeJoCo.Backend
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PhotonView))]
    public class PlayerControlled : SystemBehaviour
    {
        public PhotonView PhotonView { get; private set; }
        public Player Player { get; private set; }

        private List<IPlayerObserved> observedComponents;
        private Dictionary<byte, ISyncVar> observedVariables;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            Player = playerManager.GetPlayer(PhotonView.Controller.UserId);

            FindObserved();

            string prefix = string.Empty;
            if (Player.IsLocalPlayer)
                prefix = "*";
            this.gameObject.name = $"{prefix}({Player.NickName}) {this.gameObject.name}";
        }
        private void FindObserved()
        {
            observedComponents = new List<IPlayerObserved>();
            observedVariables = new Dictionary<byte, ISyncVar>();

            foreach (var curComponet in GetComponentsInChildren<IPlayerObserved>())
            {
                AddObservedComponent(curComponet);
            }
        }
        
        public void AddObservedComponent(IPlayerObserved component)
        {
            observedComponents.Add(component);
            component.Initialize(this);
            FindObservedFields(component);
        }
        private void FindObservedFields(IPlayerObserved component)
        {
            var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var curField in fields)
            {
                if (!curField.FieldType.GetInterfaces().Contains(typeof(ISyncVar)))
                    continue;

                var syncVar = curField.GetValue(component) as ISyncVar;

                if (!syncVar.UniqueId.HasValue)
                {
                    Debug.Log($"Field {curField.Name} of component {(component as Component).name} is not initalized and cannot be observed");
                    continue;
                }

                AddSyncVar(syncVar);
            }
        }
        public void AddSyncVar(ISyncVar syncVar)
        {
            observedVariables.Add(syncVar.UniqueId.Value, syncVar);
            syncVarHub.RegisterSyncVar(Player, syncVar);
        }
    }
}