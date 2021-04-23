using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace BiReJeJoCo.Backend
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PhotonView))]
    public class PlayerControlled : SystemBehaviour
    {
        public PhotonView PhotonView { get; private set; }
        public Player Player { get; private set; }

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            Player = playerManager.GetPlayer(PhotonView.Controller.UserId);

            Initialize();
        }

        private void Initialize()
        {
            var componentsToInitialize = LoadControlledComponents();

            foreach (var curComponent in componentsToInitialize)
            {
                curComponent.Initialize(Player);
            }
        }

        private IPlayerControlled[] LoadControlledComponents()
        {
            var components = GetComponents<IPlayerControlled>().ToList();
            foreach (var curComponet in GetComponentsInChildren<IPlayerControlled>())
            {
                components.Add(curComponet);
            }

            return components.ToArray();
        }
    }
}