using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public class CharacterSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Components")]
        [SerializeField] SynchedTransform syncedTransform;
        [SerializeField] Material hunterMat;
        [SerializeField] Material huntedMat;

        [Header("Setup")]
        [SerializeField] int fpsLayer;
        [SerializeField] LayerMask keepLayer;

        [Header("Runtime")]
        [SerializeField] Camera cam;
        [SerializeField] CharacterControllerSetup controllerSetup;
        [SerializeField] GameObject fogUpstairs;
        [SerializeField] GameObject fogDownstairs;

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

            SetTagRecursively(this.gameObject, Owner.IsLocalPlayer ? "LocalPlayer" : "RemotePlayer");
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

            SetLayerRecursively(this.gameObject, 6);
        }

        private void SpawnModel(string prefab)
        {
            var modelPrefab = MatchPrefabMapping.GetMapping().GetElementForKey(prefab);
            var newModel = Instantiate(modelPrefab, transform.position, transform.rotation, transform);
            controllerSetup = newModel.GetComponent<CharacterControllerSetup>();

            syncedTransform.SetMovementTarget(ControllerSetup.CharacterRoot);
            syncedTransform.SetRotationTarget(ControllerSetup.ModelRoot);
            syncedTransform.SetRigidBody(ControllerSetup.RigidBody);

            controller.AddObservedGameObject(newModel);
        }

        #region Access
        public Camera Cam => cam;
        public CharacterControllerSetup ControllerSetup => controllerSetup;
        public GameObject FogUpstairs => fogUpstairs;
        public GameObject FogDownstairs => fogDownstairs;
        public SynchedTransform SyncedTransform => syncedTransform;
        #endregion

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

        #region UI
        private void Update()
        {
            uiManager.GetInstanceOf<GameUI>().UpdateFloorName(matchHandler.MatchConfig.mapConfig.GetFloorName(controllerSetup.CharacterRoot.position));
        }
        #endregion

        #region Helper
        private void SetLayerRecursively(GameObject target, int layer)
        {
            if (keepLayer != (keepLayer | (1 << target.layer)))
            {
                if (target.tag == "FPS" && Owner.IsLocalPlayer)
                {
                    target.layer = fpsLayer;
                }
                else
                {
                    target.layer = layer;
                }
            }

            foreach (Transform child in target.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private void SetTagRecursively(GameObject target, string tag)
        {
            if (target.tag != "MainCamera")
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