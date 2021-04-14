namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Data part of the entity
    /// </summary>
    public interface IEntityModel
    {
        IEntity Owner { get; }
        void SetOwner(IEntity entity);

        void OnRelease();
        void OnDestroy();
    }
}