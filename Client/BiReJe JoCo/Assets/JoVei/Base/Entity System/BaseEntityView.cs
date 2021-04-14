using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Basic implementation for entity views
    /// </summary>
    public abstract class BaseEntityView<TConfig> : MonoBehaviour, IEntityView, ITickable
        where TConfig : IEntityConfig, new()
    {
        public TConfig Config { get; protected set; }
        public IEntity Owner { get; protected set; }

        public virtual void SetOwner(IEntity entity)
        {
            Owner = entity;
        }
        public virtual void Initialize(TConfig config)
        {
            Config = config;

            OnInitialize();
        }

        public virtual void OnInitialize() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnRelease() { }
        public virtual void OnDestroy() { }
    }
}