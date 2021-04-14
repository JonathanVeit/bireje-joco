namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Visual part of the entity
    /// </summary>
    public interface IEntityView
    {
        IEntity Owner { get; }
        void SetOwner(IEntity entity);

        void OnRelease();
        void OnDestroy();
    }
}