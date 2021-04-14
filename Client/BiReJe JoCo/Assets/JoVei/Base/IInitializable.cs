using System.Collections;

namespace JoVei.Base
{
    /// <summary>
    /// A system which can be initialized
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Async initialization
        /// </summary>
        IEnumerator Initialize(object[] parameters);

        /// <summary>
        /// Cleanup after destroying the system
        /// </summary>
        void CleanUp();
    }
}