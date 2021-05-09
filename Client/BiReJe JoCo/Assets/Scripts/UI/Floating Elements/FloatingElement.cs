using JoVei.Base.UI;
using JoVei.Base;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class FloatingElement : BaseFloatingElement, ITickable
    {
        private MeshRenderer visibleRenderer;
        private bool isHided;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            tickSystem.Register(this);
        }

        protected override void OnBeforeDestroy()
        {
            tickSystem.Unregister(this);
        }

        public void SetVisibleRenderer(MeshRenderer renderer)
        {
            visibleRenderer = renderer;
        }

        public virtual void Tick (float deltaTime)
        {
            if (visibleRenderer != null && !isHided)
            {
                if (transform.gameObject.activeSelf != visibleRenderer.isVisible)
                    transform.gameObject.SetActive(visibleRenderer.isVisible);
            }
        }

        public void Hide() 
        {
            this.gameObject.SetActive(false);
            isHided = true;
        }
        public void Unhide() 
        {
            this.gameObject.SetActive(true);
            isHided = false;
        }
    }
}
