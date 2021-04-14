namespace JoVei.Base
{
    /// <summary>
    /// Class with access to all systems
    /// </summary>
    public abstract class BaseSystemAccessor
    {
        protected static ISystemsLoader systemsInitializer => DIContainer.GetImplementationFor<ISystemsLoader>();
        protected static TickSystem.ITickSystem tickSystem => DIContainer.GetImplementationFor<TickSystem.ITickSystem>();
        protected static Data.IGameDataManager gameDataManager => DIContainer.GetImplementationFor<Data.IGameDataManager>();
        protected static Data.IInstanceFactory instanceFactory => DIContainer.GetImplementationFor<Data.IInstanceFactory>();
        protected static IGlobalVariableStorage globalVariables => DIContainer.GetImplementationFor<IGlobalVariableStorage>();
        protected static ILocalizer localizer => DIContainer.GetImplementationFor<ILocalizer>();
        protected static Helper.IDebugController debugController => DIContainer.GetImplementationFor<Helper.IDebugController>();
        protected static PoolingSystem.IPoolingManager poolingManager => DIContainer.GetImplementationFor<PoolingSystem.IPoolingManager>();
        protected static EntitySystem.IEntityControlSystem ECS => DIContainer.GetImplementationFor<EntitySystem.IEntityControlSystem>();
        protected static Data.IProfileHandler profileHandler => DIContainer.GetImplementationFor<Data.IProfileHandler>();
        protected static UI.IFloatingElementManager floatingManager => DIContainer.GetImplementationFor<UI.IFloatingElementManager>();
        protected static MessageSystem.IMessageHub messageHub => DIContainer.GetImplementationFor<MessageSystem.IMessageHub>();
    }
}