using JoVei.Base.UI;

namespace BiReJeJoCo
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