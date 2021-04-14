using UnityEngine;
using JoVei.Base.Helper;

namespace JoVei.Base.PoolingSystem
{
    /// <summary>
    /// SystemBehaviour that can be dynamically pooled 
    /// Can be used as parent class or single component on prefabs
    /// </summary>
    [DisallowMultipleComponent]
    public class PoolablePrefab : BaseSystemBehaviour, IPoolable
    {
        #region Inspector
        [SerializeField] private InspectorDrawer poolSettings;

        [System.Serializable]
        private class InspectorDrawer
        {
            public string poolId = null;
            public PoolType Type = PoolType.Dynamic;
            public int poolLimit = 10;

            public InspectorDrawer() 
            {
                if (string.IsNullOrEmpty(poolId))
                    poolId = IdentificationHelper.GetUniqueIdentifier();
            }
        }
        #endregion

        #region Exposed Member
        /// <summary>
        /// Unique Identifier of the pool
        /// </summary>
        public string PoolId { get { return poolSettings.poolId; } }
        /// <summary>
        /// Type of the pool
        /// </summary>
        public PoolType PoolType { get { return poolSettings.Type; } }
        /// <summary>
        /// Limit of instances till repooling
        /// </summary>
        public int PoolLimit { get { return poolSettings.poolLimit; } }
        /// <summary>
        /// Root GameObject to be instantiated
        /// </summary>
        public GameObject RootObject { get { return this.gameObject; } }

        /// <summary>
        /// Request returning the the instance to its pool
        /// </summary>
        public void RequestReturnToPool()
        {
            poolingManager.Return(this);
        }

        /// <summary>
        /// Request releasing the instance from its pool
        /// </summary>
        public void RequestReleaseFromPool() 
        {
            poolingManager.Release(this);
        }
        #endregion
    }
}