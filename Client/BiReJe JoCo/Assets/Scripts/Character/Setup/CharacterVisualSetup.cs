using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class CharacterVisualSetup : SystemBehaviour, IPlayerObserved
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
                if (curSetup.renderer)
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
                if (curSetup.renderer2)
                {
                    var materials = curSetup.renderer2.materials;
                    if (Owner.NumberInRoom > curSetup.materials.Length - 1)
                    {
                        materials[curSetup.materialIndex] = curSetup.materials[0];
                        continue;
                    }
                    materials[curSetup.materialIndex] = curSetup.materials[Owner.NumberInRoom];

                    curSetup.renderer2.materials = materials;
                }
            }
        }

        [System.Serializable]
        private struct ColorSetup
        {
            public SkinnedMeshRenderer renderer;
            public MeshRenderer renderer2;
            public int materialIndex;
            public Material[] materials;
        }
    }
}