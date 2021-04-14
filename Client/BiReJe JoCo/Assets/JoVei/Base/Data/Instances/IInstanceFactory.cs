using System.Collections;
using JoVei.Base.Helper;

namespace JoVei.Base.Data
{
    /// <summary>
    /// Factory to create instances
    /// </summary>
    public interface IInstanceFactory 
    {
        TInstance CreateInstance<TInstance>()
            where TInstance : IInstance, new();
    }
}