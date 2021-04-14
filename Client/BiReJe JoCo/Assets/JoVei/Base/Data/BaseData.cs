using System.Collections.Generic;
using System.Linq;

namespace JoVei.Base.Data
{
    /// <summary>
    /// The base for all kind of deserialized data 
    /// </summary>
    public abstract class BaseData : BaseSystemAccessor, IUniqueId
    {
        public string UniqueId { get; set; }
    }
}