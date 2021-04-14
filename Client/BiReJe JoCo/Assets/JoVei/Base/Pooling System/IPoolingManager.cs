using UnityEngine;

namespace JoVei.Base.PoolingSystem
{
    /// <summary>
    /// Manages pooling for all kind of prefabs
    /// </summary>
    public interface IPoolingManager
    {
        // Getting instances
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab, Vector3 position, Quaternion rotation)
            where TPoolable : IPoolable;
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab, Transform parent)
            where TPoolable : IPoolable;
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab)
            where TPoolable : IPoolable;

        public IPoolable PoolInstance(IPoolable prefab, Vector3 position, Quaternion rotation);
        public IPoolable PoolInstance(IPoolable prefab, Transform parent);
        public IPoolable PoolInstance(IPoolable prefab);

        public GameObject PoolInstance(GameObject prefab, Vector3 position, Quaternion rotation);
        public GameObject PoolInstance(GameObject prefab, Transform parent);
        public GameObject PoolInstance(GameObject prefab);

        // helper
        public bool IsPoolable(object prefab);
         
        // deturning instances
        public void Return(IPoolable instance); // instance 
        public void Return(GameObject instance); // instance 
        public void ReturnAll(string poolId); // of pool

        // deleasing instances 
        public void Release(IPoolable instance); // instance
        public void Release(GameObject instance); // instance

        public void ReleaseAll(string poolId); // of pool

        // destroying instances
        public void Destroy(IPoolable instance);
        public void Destroy(GameObject instance);
        public void DestroyAll(string poolId);
    }
}