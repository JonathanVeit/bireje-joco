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
        private Dictionary<byte, string> variableCache;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            Player = playerManager.GetPlayer(PhotonView.Controller.UserId);

            FindObserved();
            InitializeComponents();
            InitializeVariables();

            this.gameObject.name = $"({Player.NickName}) Player Character";
        }

        private void InitializeComponents()
        {
            foreach (var curComponent in observedComponents)
            {
                curComponent.Initialize(Player);
            }
        }

        private void InitializeVariables() 
        {
            foreach (var curVariable in observedVariables.Values)
            {
                syncVarHub.RegisterSyncVar(Player, curVariable);
            }
        }

        private void FindObserved()
        {
            observedComponents = new List<IPlayerObserved>();
            observedVariables = new Dictionary<byte, ISyncVar>();
            variableCache = new Dictionary<byte, string>();

            foreach (var curComponet in GetComponentsInChildren<IPlayerObserved>())
            {
                if (!observedComponents.Contains(curComponet))
                    observedComponents.Add(curComponet);

                FindObservedField(curComponet);
            }
        }

        private void FindObservedField(IPlayerObserved component)
        {
            var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var curField in fields)
            {
                if (!curField.FieldType.GetInterfaces().Contains(typeof(ISyncVar)))
                    continue;

                var syncVar = curField.GetValue(component) as ISyncVar;

                if (!syncVar.UniqueId.HasValue)
                {
                    Debug.LogError($"Field {curField.Name} of component {(component as Component).name} is not initalized and cannot be observed");
                    continue;
                }

                observedVariables.Add(syncVar.UniqueId.Value, syncVar);
            }
        }
    }
}