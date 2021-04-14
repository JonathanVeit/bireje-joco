using System;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Smallest entity
    /// </summary>
    public interface IEntity
    {
        event Action<IEntity> onRelease;
        event Action<IEntity> onDestroy;

        // unique identifier
        string EntityId { get; }

        // id used to created
        string PrefabId  { get; }

        // system owning the entity
        IEntityControlSystem System { get; }

        // initalization
        void Initialize(string entityId, IEntitySpawnConfig spawnConfig, IEntityControlSystem owningSystem);

        // release from owning system -> to system
        void RequestRelease();

        // destroy completly -> to system
        void RequestDestroy();

        // before releasing
        void Release();

        // before destroying
        void Destroy();
    }
}