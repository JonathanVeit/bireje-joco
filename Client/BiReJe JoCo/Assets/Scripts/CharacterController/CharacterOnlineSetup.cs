using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;
using Unity.VisualScripting;

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
        [SerializeField] public GameObject fogUpstairs;
        [SerializeField] public GameObject fogDownstairs;
        [SerializeField] public GameObject fogDownstairsHunted;
        [SerializeField] public GameObject fogUpstairsHunted;
        [SerializeField] List<MonoBehaviour> componentsToDisable;
        [SerializeField] GameObject huntedHead;

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
            fogUpstairs.SetActive(false);
            fogDownstairs.SetActive(false);
            fogDownstairsHunted.SetActive(false);
            fogUpstairsHunted.SetActive(false);

            foreach (var curComponent in componentsToDisable)
                curComponent.enabled = false;
            rb.isKinematic = true;
            SetTagRecursively(this.gameObject, "RemotePlayer");
        }

        private void SetupAsHunted() 
        {
            var model = characterRoot.AddComponent<HuntedCharacterModel>();
            controller.AddObservedComponent(model);

            // TODO: hunted should not have a lamp but better pp 
            if (!Owner.IsLocalPlayer)
                GetComponentsInChildren<Light>()[0].gameObject.SetActive(false);

            SetLayerRecursively(this.gameObject, 10);
            huntedHead.gameObject.SetActive(true);
        }

        private void SetupAsHunter() 
        {
            var gunPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunter_gun");

            var root = characterRoot.transform.GetChild(0);
            var gun = Instantiate(gunPrefab, root.position, Quaternion.identity, root);
            controller.AddObservedComponent(gun.GetComponent<CharacterShooter>());
        }

        #region Helper
        private void SetLayerRecursively(GameObject target, int layer)
        {
            target.layer = layer;

            foreach (Transform child in target.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private void SetTagRecursively(GameObject target, string tag)
        {
            target.tag = tag;

            foreach (Transform child in target.transform)
            {
                SetTagRecursively(child.gameObject, tag);
            }
        }
        #endregion
    }
}
