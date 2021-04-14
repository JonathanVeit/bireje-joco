using System;
using JoVei.Base.PoolingSystem;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Basic implementation for entities with view and controller as PoolablePrefab
    /// </summary>
    public abstract class BaseComponentEntity<TConfig, TController, TView, TModel> : PoolablePrefab, IEntity, ITickable
        where TConfig : IEntityConfig, new()
        where TController : BaseEntityController<TConfig>
        where TView : BaseEntityView<TConfig>
        where TModel : BaseEntityModel<TConfig>
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
        public string PrefabId { get; private set; }

        /// <summary>
        /// Currently owning system
        /// </summary>
        public IEntityControlSystem System { get; protected set; }

        /// <summary>
        /// Controller component
        /// </summary>
        public TController Controller { get; protected set; }

        /// <summary>
        /// View component 
        /// </summary>
        public TView View { get; protected set; }

        /// <summary>
        /// Model component 
        /// </summary>
        public TModel Model { get; protected set; }

        /// <summary>
        /// Config
        /// </summary>
        public TConfig Config { get; protected set; }

        public virtual void Initialize(string entityId, IEntitySpawnConfig spawnConfig, IEntityControlSystem owningSystem)
        {
            // store config 
            Config = (TConfig)spawnConfig.EntityConfig;
            EntityId = entityId;
            PrefabId  = spawnConfig.PrefabId;
            System = owningSystem;

            // create controller
            Controller = CreateController();
            Controller.SetOwner(this);

            // create view
            View = CreateView();
            View.SetOwner(this);

            // create model 
            Model = CreateModel();
            Model.SetOwner(this);

            // initialzie self 
            OnInitialize();

            // initialize components 
            Controller.Initialize(Config);
            View.Initialize(Config);
            Model.Initialize(Config);
        }

        #region Abstract Member
        protected abstract TController CreateController();

        protected abstract TView CreateView();

        protected abstract TModel CreateModel();
        #endregion

        #region Base Behaviour
        /// <summary>
        /// Called right after all components have been created, but before they are beeing initialized
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// Tick Update called by the ECS 
        /// </summary>
        public virtual void Tick(float deltaTime)
        {
            Controller.Tick(deltaTime);
            View.Tick(deltaTime);
        }

        /// <summary>
        /// Request destroying the entity by the ECS 
        /// </summary>
        public virtual void RequestDestroy()
        {
            System.DestroyEntity(this);
        }

        /// <summary>
        /// Request releasing the entity by the ECS 
        /// </summary>
        public virtual void RequestRelease()
        {
            System.ReleaseEntity(this);
        }

        /// <summary>
        /// Release called by the ECS 
        /// </summary>
        public virtual void Release()
        {
            Controller.OnRelease();
            View.OnRelease();
            Model.OnRelease();
            onRelease?.Invoke(this);
        }

        /// <summary>
        /// Destroy called by the ECS 
        /// </summary>
        public virtual void Destroy()
        {
            Controller.OnDestroy();
            View.OnDestroy();
            Model.OnDestroy();
            onDestroy?.Invoke(this);
        }
        #endregion
    }
}