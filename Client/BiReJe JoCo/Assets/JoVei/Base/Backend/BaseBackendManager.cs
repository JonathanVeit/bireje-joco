using System.Collections;

namespace JoVei.Base.Backend
{
    /// <summary>
    /// Default Backend Manager that loades an backend api and registers itself at the DI container
    /// </summary>
    public abstract class BaseBackendManager : BaseSystemAccessor, IBackendManager
    {
        public abstract IBaseBackendAPI backendAPI { get; set; }

        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IBackendManager>(this);

            LoadAPI();
            yield return backendAPI.Login();
        }

        public virtual void CleanUp()
        {
        }

        public abstract void LoadAPI();
    }
}