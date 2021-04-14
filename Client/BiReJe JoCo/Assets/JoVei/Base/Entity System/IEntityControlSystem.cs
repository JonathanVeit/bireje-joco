using System.Collections.Generic;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Controls the livecycle of entities
    /// </summary>
    public interface IEntityControlSystem : ITickable
    {
        List<IEntity> Entities { get; }

        // add
        IEntity CreateEntity(IEntitySpawnConfig config);  // create entity, probbaly instantiate or pooling
        TEntity CreateEntityAs<TEntity>(IEntitySpawnConfig config);  // create entity, probbaly instantiate or pooling
        void AddEntity(IEntity entity); // add an existing entitiy 

        // remove  
        void ReleaseEntity(IEntity entity); // remove entity, dont destroy
        void ReleaseAll(); // remove entity, dont destroy all entities

        void DestroyEntity(IEntity entity); // remove and destroy entity
        void DestroyAll(); // remove and destroy all entities
    }
}