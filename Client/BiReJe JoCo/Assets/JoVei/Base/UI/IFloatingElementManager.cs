using UnityEngine;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Manager for floating elements of all kind
    /// </summary>
    public interface IFloatingElementManager
    {
        IFloatingElement GetElement(IFloatingElementConfig config); // create new element and return
        TElement GetElementAs<TElement>(IFloatingElementConfig config) where TElement : UnityEngine.Object, IFloatingElement; // create new element and return as

        void ReleaseElement(IFloatingElement element); // release one but dont destroy
        void ReleaseAll(); // release all

        void DestroyElement(IFloatingElement element); // release and destroy
        void DestroyElement(Transform target); // release and destroy
        void DestroyAll(); // release and destroy all
    }
}