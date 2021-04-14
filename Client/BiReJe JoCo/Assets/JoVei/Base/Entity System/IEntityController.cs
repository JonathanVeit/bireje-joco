namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Logic part of the entitiy
    /// </summary>
    public interface IEntityController
    {
        IEntity Owner { get; }
        void SetOwner(IEntity entity);

        void OnRelease();
        void OnDestroy();
    }
}