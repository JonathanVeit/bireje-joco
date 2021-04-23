using JoVei.Base.UI;
using JoVei.Base;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class FloatingElement : BaseFloatingElement, ITickable
    {
        private MeshRenderer visibleRenderer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            tickSystem.Register(this);
        }

        protected override void OnBeforeDestroy()
        {
            tickSystem.Unregister(this);
        }

        public void SetVisibleMesh(MeshRenderer renderer)
        {
            visibleRenderer = renderer;
        }

        public void Tick (float deltaTime)
        {
            if (visibleRenderer != null)
            {
                if (transform.gameObject.activeSelf != visibleRenderer.isVisible)
                    transform.gameObject.SetActive(visibleRenderer.isVisible);
            }
        }
    }
}
