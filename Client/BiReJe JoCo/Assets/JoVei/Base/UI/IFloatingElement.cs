namespace JoVei.Base.UI
{
    public interface IFloatingElement
    {
        IFloatingElementConfig Config { get; } // config 
        UnityEngine.Transform FloatyRoot { get; }
        void Initialize(IFloatingElementConfig config); // initialize when pooling

        void RequestDestroyFloaty();
        void RequestReleaseFloaty();
    }
}