using UnityEngine;

namespace JoVei.Base.PoolingSystem
{
    /// <summary>
    /// Settings to specifiy pools
    /// </summary>
    public interface IPoolable
    { 
        public string PoolId { get; } 
        public PoolType PoolType { get; }
        public int PoolLimit { get; }
        public GameObject RootObject { get; }

        public void RequestReturnToPool();
        public void RequestReleaseFromPool();
    }
}