using System;
using JoVei.Base.PoolingSystem;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Basic implementation for entities as PoolablePrefab
    /// </summary>
    public abstract class BaseEntity<TConfig>: PoolablePrefab, IEntity, ITickable
        where TConfig : IEntityConfig
    {
        /// <summary>
        /// Called right before beeing release from the system
        /// </summary>
        public event Action<IEntity> onRelease;

        /// <summary>
        /// Called right before beeing destroyed/returned to the pool
        /// </summary>
        public event Action<IEntity> onDestroy;

        /// <summary>
        /// Unique identifier 
        /// </summary>
        public string EntityId { get; private set; }

        /// <summary>
        /// Unique identifier 
        /// </summary>
        public string PrefabId  { get; private set; }

        /// <summary>
        /// Config
        /// </summary>
        public TConfig Config { get; protected set; }

        /// <summary>
        /// Currently owning system
        /// </summary>
        public IEntityControlSystem System { get; protected set; }

        public virtual void Initialize(string entityId, IEntitySpawnConfig spawnConfig, IEntityControlSystem owningSystem)
        {
            // store config 
            Config = (TConfig)spawnConfig.EntityConfig;
            EntityId = entityId;
            PrefabId  = spawnConfig.PrefabId;
            System = owningSystem;

            OnInitialize();
        }

        #region Base Behaviour
        public virtual void OnInitialize() { }

        public virtual void Tick(float deltaTime)
        {
        }

        public virtual void RequestDestroy()
        {
            System.DestroyEntity(this);
        }

        public virtual void RequestRelease()
        {
            System.ReleaseEntity(this);
        }

        public virtual void Release()
        {
            onRelease?.Invoke(this);
        }

        public virtual void Destroy()
        {
            onDestroy?.Invoke(this);
        }
        #endregion
    }
}