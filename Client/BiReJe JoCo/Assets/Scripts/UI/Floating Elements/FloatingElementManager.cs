using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    /// <summary>
    /// Our implemenation for the FloatingElementManager
    /// </summary>
    public class FloatingElementManager : BaseFloatingElementManager
    {
        protected override IFloatingElementFactory CreateFactory()
        {
            return new FloatingElementFactory();
        }


    }
}