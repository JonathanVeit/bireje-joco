using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class CharacterOnlineSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] GameObject characterRoot;

        [Space(10)]
        [SerializeField] private GameObject cam;
        [SerializeField] private GameObject cinemachineObject;
        [SerializeField] private Rigidbody rb;
        [SerializeField] List<MonoBehaviour> componentsToDisable;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
            {
                SetupAsRemote();
            }

            switch (Owner.Role)
            {
                case PlayerRole.Hunted:
                    SetupAsHunted();
                    break;
                case PlayerRole.Hunter:
                    SetupAsHunter();
                    break;
            }
        }

        private void SetupAsRemote()
        {
            cam.SetActive(false);
            cinemachineObject.SetActive(false);

            foreach (var curComponent in componentsToDisable)
                curComponent.enabled = false;
            rb.isKinematic = true;
        }

        private void SetupAsHunted() 
        {
            var model = characterRoot.AddComponent<HuntedCharacterModel>();
            controller.AddObservedComponent(model);
        }

        private void SetupAsHunter() 
        {
            var gunPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunter_gun");

            var root = characterRoot.transform.GetChild(0);
            var gun = Instantiate(gunPrefab, root.position, Quaternion.identity, root);
            controller.AddObservedComponent(gun.GetComponent<CharacterShooter>());
        }
    }
}
