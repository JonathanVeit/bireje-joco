using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JoVei.Base.Data
{
    /// <summary>
    /// Can be used to store instances 
    /// </summary>
    public class InstanceHandler<TInstance> : BaseSystemAccessor
        where TInstance : IInstance
    {
        // all registered instances
        [JsonProperty] private Dictionary<string, TInstance> instances;
     
        public TInstance this[string instanceId]
        {
            get 
            {
                CheckAndCreateInstances();
                if (instances.ContainsKey(instanceId))
                    return instances[instanceId];
                else
                    throw new KeyNotFoundException(string.Format("Cannot find instance with Id {0}", instanceId));
            }
            set
            {
                CheckAndCreateInstances();
                if (instances.ContainsKey(instanceId))
                    instances[instanceId] = value;
                else
                    throw new KeyNotFoundException(string.Format("Cannot find instance with Id {0}", instanceId));
            }
        }

        public void Add(TInstance instance)
        {
            CheckAndCreateInstances();
            instances.Add(instance.InstanceId, instance);
        }

        public TInstance[] GetAll() 
        {
            CheckAndCreateInstances();

            return instances.Values.ToArray();
        }

        public string[] GetAllIds() 
        {
            CheckAndCreateInstances();
            return instances.Keys.ToArray();
        }

        public bool HasInstanceWithId(string instanceId)
        {
            CheckAndCreateInstances();
            return instances.ContainsKey(instanceId);
        }

        public void Remove(string instanceId)
        {
            CheckAndCreateInstances();
            instances.Remove(instanceId);
        }

        public void Clear()
        {
            CheckAndCreateInstances();
            instances.Clear();
        }

        #region Helper
        private void CheckAndCreateInstances() 
        {
            if (instances == null) instances = new Dictionary<string, TInstance>();
        }
        #endregion
    }
}