using System.Collections;
using System.Collections.Generic;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Basic implementation of an entity controlling system
    /// </summary>
    public abstract class BaseEntityControlSystem : IEntityControlSystem, ITickable, IInitializable
    {
        /// <summary>
        /// All IEntity currently controlled by the system
        /// </summary>
        public virtual List<IEntity> Entities { get; protected set; }
            = new List<IEntity>();
        protected IEntityFactory factory;

        #region Initialziation
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IEntityControlSystem>(this);
            factory = CreateFactory();
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        #region Add Entities
        /// <summary>
        /// Instantiates a new IEntity to be controlled by the system
        /// </summary>
        public virtual TEntity CreateEntityAs<TEntity>(IEntitySpawnConfig config)
        {
            return (TEntity) CreateEntity(config);
        }

        /// <summary>
        /// Instantiates a new IEntity to be controlled by the system
        /// </summary>
        public virtual IEntity CreateEntity(IEntitySpawnConfig config)
        {
            // spawn entity
            var newEntity = factory.CreateEntityForConfig(config);

            newEntity.Initialize(NewEntityId(), config, this);
            AddEntity(newEntity);
            return newEntity;
        }

        /// <summary>
        /// Adds an existing IEntity to be controlled by the system
        /// </summary>
        public virtual void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
        }
        #endregion

        #region Remove Entities
        /// <summary>
        /// The IEntity will no longer be controlled by the system
        /// </summary>
        public virtual void ReleaseEntity(IEntity entity)
        {
            Entities.Remove(entity);
            entity.Release();
        }

        /// <summary>
        /// The Entity will be destroyed in the implemented way
        /// </summary>
        public virtual void DestroyEntity(IEntity entity)
        {
            Entities.Remove(entity);
            entity.Destroy();
            factory.DestroyEntity(entity);
        }

        /// <summary>
        /// All entities will be destroyed in the implemetned way
        /// </summary>
        public virtual void ReleaseAll()
        {
            foreach (var curEntity in Entities.ToArray())
            {
                ReleaseEntity(curEntity);
            }
        }

        /// <summary>
        /// All entities will be destroyed in the implemetned way
        /// </summary>
        public virtual void DestroyAll()
        {
            foreach (var curEntity in Entities.ToArray())
            {
                DestroyEntity(curEntity);
            }
        }
        #endregion

        #region Behaviour
        public virtual void Tick(float deltaTime)
        {
            foreach (var curEntity in Entities.ToArray())
            {
                if (curEntity.Equals(null)) continue;

                var tickable = (curEntity as ITickable);
                if (tickable != null) tickable.Tick(deltaTime);
            }
        }
        #endregion

        #region Helper
        private int totalEntityCount = 0;
        protected virtual string NewEntityId()
        {
            totalEntityCount++;
            return "entity_" + totalEntityCount;
        }

        protected abstract IEntityFactory CreateFactory();
        #endregion
    }
}