using JoVei.Base;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    /// <summary>
    /// Our implemenation for the FloatingElementManager
    /// </summary>
    public class FloatingElementManager : BaseFloatingElementManager, ITickable
    {
        protected override void OnSystemsInitialized()
        {
            tickSystem.Register(this, "late_update");
        }
        protected override void OnBeforeDestroy()
        {
            tickSystem.Unregister(this);
        }
        protected override IFloatingElementFactory CreateFactory()
        {
            return new FloatingElementFactory();
        }

        protected override void Update() { }
        public void Tick(float deltaTime)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                UpdateElement(Elements[i]);
            }
        }
    }
}