using System;
using System.Collections.Generic;

namespace JoVei.Base.Data
{
    /// <summary>
    /// DataCenter contains all container for relevaten player data 
    /// Only one container per type is allowed 
    /// </summary>
    public abstract class DataCenter<TDataKey> : BaseSystemAccessor
        where TDataKey : Enum
    {
        [Newtonsoft.Json.JsonProperty] protected Dictionary<TDataKey, object> RegisteredContainer { get; set; }
            = new Dictionary<TDataKey, object>();

        public void RegisterDataContainer(TDataKey type, object container)
        {
            if (RegisteredContainer.ContainsKey(type))
                throw new ArgumentException(string.Format("There is already a container registered for data type {0}", type));

            RegisteredContainer.Add(type, container);
        }

        public void UnregisterContainer(TDataKey type)
        {
            if (!RegisteredContainer.ContainsKey(type))
                throw new ArgumentException(string.Format("There is no container registered for data type {0}", type));

            RegisteredContainer.Remove(type);
        }

        public TValue GetDataContainer<TValue>(TDataKey type)
        {
            if (!RegisteredContainer.ContainsKey(type))
                throw new ArgumentException(string.Format("No data container registered for data type {0}", type));

            if (RegisteredContainer[type] is TValue targetValue)
                return targetValue;

            throw new ArgumentException(string.Format("Registered container for data type {0} cannot be casted into {1} because its of type {2}", 
                type, 
                typeof(TValue), 
                RegisteredContainer[type].GetType()));
        }
    }
}
