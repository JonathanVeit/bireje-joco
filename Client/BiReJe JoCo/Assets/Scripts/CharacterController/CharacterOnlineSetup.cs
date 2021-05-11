using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class CharacterOnlineSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Components")]
        public Transform characterRoot;
        public Transform modelRoot;
        public GameObject cam;
        public Rigidbody rb;
        public Light flashlight;
        public PlayerCharacterInput characterInput;

        [Header("Remote Settings")]
        [SerializeField] List<MonoBehaviour> componentsToDisable;
        [SerializeField] List<GameObject> objectsToDisable;
        [SerializeField] List<Object> objectsToDestroy;

        [Header("Runtime")]
        public GameObject fogUpstairs;
        public GameObject fogDownstairs;

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
                    SetupAsHunted(Owner.IsLocalPlayer);
                    break;
                case PlayerRole.Hunter:
                    SetupAsHunter(Owner.IsLocalPlayer);
                    break;
            }
        }

        private void SetupAsRemote()
        {
            foreach (var curComponent in componentsToDisable)
                curComponent.enabled = false;
            foreach (var curObject in objectsToDisable)
                curObject.SetActive(false);
            foreach (var curObject in objectsToDestroy)
                Destroy(curObject);

            rb.isKinematic = true;
            SetTagRecursively(this.gameObject, "RemotePlayer");
        }
        private void SetupAsHunted(bool isLocal)
        {
            // add hunted behaviour 
            var behaviourPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_behaviour");
            var huntedBehviour = Instantiate(behaviourPrefab, characterRoot).GetComponent<HuntedBehaviour>();
            controller.AddObservedComponent(huntedBehviour);

            // TODO: hunted should not have a lamp but better pp 
            if (isLocal)
                SpawnFlashlight();

             // spawn marker object 
             var earPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunted_ears");
            Instantiate(earPrefab, modelRoot.position, modelRoot.rotation, modelRoot);

            if (isLocal)
                SetupHuntedPP();

            // set layer to hunted layer
            SetLayerRecursively(this.gameObject, 10);
        }
        private void SetupAsHunter(bool isLocal)
        {
            // spawn gun 
            var gunPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunter_gun");
            var root = characterRoot.transform.GetChild(0);
            var gun = Instantiate(gunPrefab, root.position, Quaternion.identity, root);
            controller.AddObservedComponent(gun.GetComponent<Gun>());

            if (isLocal)
                SetupHunterPP();

            SpawnFlashlight();
        }

        private void SpawnFlashlight() 
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("flashlight");
            var root = characterRoot.transform.GetChild(0);
            var flashlight = Instantiate(prefab, root.position, Quaternion.identity, root);
            controller.AddObservedComponent(flashlight.GetComponent<Flashlight>());
        }

        #region PostProcessing
        private void SetupHuntedPP() 
        {
            var fogUpstairsPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("fog_upstairs_hunted_sfx");
            var fogDownstairsPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("fog_downstairs_hunted_sfx");
            
            var root = CreateSFXRoot();
            fogUpstairs = Instantiate(fogUpstairsPrefab, root);
            fogDownstairs = Instantiate(fogDownstairsPrefab, root);
        }

        private void SetupHunterPP() 
        {
            var fogUpstairsPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("fog_upstairs_hunter_sfx");
            var fogDownstairsPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("fog_downstairs_hunter_sfx");

            var root = CreateSFXRoot();
            fogUpstairs = Instantiate(fogUpstairsPrefab, root);
            fogDownstairs = Instantiate(fogDownstairsPrefab, root);
        }
        #endregion

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

        private Transform CreateSFXRoot() 
        {
            var sfx_root = new GameObject("SFX");
            sfx_root.transform.position = cam.transform.position;
            sfx_root.transform.rotation = cam.transform.rotation;
            sfx_root.transform.SetParent(cam.transform);
            return sfx_root.transform;
        }
        #endregion
    }
}
