using System.Collections;
using JoVei.Base.Helper;

namespace JoVei.Base.Data
{
    public class BaseInstanceFactory : IInstanceFactory, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            Setup();
            DIContainer.RegisterImplementation<IInstanceFactory>(this);
            yield return null;
        }

        protected virtual void Setup() { }

        public void CleanUp() { }
        #endregion

        /// <summary>
        /// Create an intance of the given type 
        /// </summary>
        public virtual TInstance CreateInstance<TInstance>()
            where TInstance : IInstance, new ()
        {
            var instance = new TInstance();
            var id = NewId();
            instance.Create(id);
            return instance;
        }

        #region Helper
        /// <summary>
        /// Return a new unique id 
        /// </summary>
        protected virtual string NewId() { return IdentificationHelper.GetUniqueIdentifier(); }
        #endregion
    }
}