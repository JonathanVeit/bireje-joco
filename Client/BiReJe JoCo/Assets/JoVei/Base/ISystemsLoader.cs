using System;

namespace JoVei.Base
{
    public interface ISystemsLoader
    {
        event Action OnAllSystemsLoaded;
        event Action<string> OnStartLoadingSystem;
        event Action<string> OnErrorOccured;
        event Action<object> OnSystemLoaded;

        int TotalElementsToSetup { get; }
        float CurrentProgress { get; }
        bool Finished { get; }
    }
}
