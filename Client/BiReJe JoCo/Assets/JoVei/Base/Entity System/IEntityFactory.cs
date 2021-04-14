namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Handles instantiation of entities 
    /// </summary>
    public interface IEntityFactory
    {
        IEntity CreateEntityForConfig(IEntitySpawnConfig config);

        public void DestroyEntity(IEntity entity);
    }
}