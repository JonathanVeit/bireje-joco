namespace JoVei.Base.UI
{ 
    /// <summary>
    /// Interface for an floating element factory
    /// </summary>
    public interface IFloatingElementFactory
    {
        public IFloatingElement CreateElementForConfig(IFloatingElementConfig config);

        public void DestroyElement(IFloatingElement element);
    }
}
