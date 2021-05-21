using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;

namespace BiReJeJoCo.Character
{
    public class CharacterSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Components")]
        public SynchedTransform syncedTransform;
        public Material hunterMat;
        public Material huntedMat;

        [Header("Runtime")]
        public Camera cam;
        public CharacterControllerSetup controllerSetup;
        public GameObject fogUpstairs;
        public GameObject fogDownstairs;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            Owner.AssignCharacter(this);

            if (Owner.IsLocalPlayer)
            {
                cam = Camera.main;
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

        private void SetupAsHunted()
        {
            if (!Owner.IsLocalPlayer)
            {
                SpawnModel("hunted_model_remote");
            }
            else
            {
                SpawnModel("hunted_model_local");
                SetupHuntedPP();
            }

            // set layer to hunted layer
            SetLayerRecursively(this.gameObject, 10);
        }
        private void SetupAsHunter()
        {
            if (!Owner.IsLocalPlayer)
            {
                SpawnModel("hunter_model_remote");
            }
            else
            {
                SpawnModel("hunter_model_local");
                SetupHunterPP();
            }
        }

        private void SpawnModel(string prefab)
        {
            var modelPrefab = MatchPrefabMapping.GetMapping().GetElementForKey(prefab);
            var newModel = Instantiate(modelPrefab, transform.position, transform.rotation, transform);
            controllerSetup = newModel.GetComponent<CharacterControllerSetup>();

            syncedTransform.SetMovementTarget(controllerSetup.characterRoot);
            syncedTransform.SetRotationTarget(controllerSetup.modelRoot);
            syncedTransform.SetRigidBody(controllerSetup.rigidBody);

            controller.AddObservedGameObject(newModel);
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