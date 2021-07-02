using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class CharacterColorSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] ColorSetup[] colorSetups;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            SetColors();
        }
        private void SetColors() 
        {
            foreach (var curSetup in colorSetups)
            {
                var materials = curSetup.renderer.materials;
                if (Owner.NumberInRoom > curSetup.materials.Length - 1)
                {
                    materials[curSetup.materialIndex] = curSetup.materials[0];
                    continue;
                }
                materials[curSetup.materialIndex] = curSetup.materials[Owner.NumberInRoom];

                curSetup.renderer.materials = materials;
            }
        }

        [System.Serializable]
        private struct ColorSetup
        {
            public SkinnedMeshRenderer renderer;
            public int materialIndex;
            public Material[] materials;
        }
    }
}