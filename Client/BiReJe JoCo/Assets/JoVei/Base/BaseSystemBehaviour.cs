using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Monobehaviour with access to all important systems
    /// </summary>
    public abstract class BaseSystemBehaviour : MonoBehaviour
    {
        protected static ISystemsLoader systemsInitializer => DIContainer.GetImplementationFor<ISystemsLoader>();
        protected static TickSystem.ITickSystem tickSystem => DIContainer.GetImplementationFor<TickSystem.ITickSystem>();
        protected static Data.IGameDataManager dataManager => DIContainer.GetImplementationFor<Data.IGameDataManager>();
        protected static Data.IInstanceFactory instanceFactory => DIContainer.GetImplementationFor<Data.IInstanceFactory>();
        protected static IGlobalVariableStorage globalVariables => DIContainer.GetImplementationFor<IGlobalVariableStorage>();
        protected static ILocalizer localizer => DIContainer.GetImplementationFor<ILocalizer>();
        protected static Helper.IDebugController debugController => DIContainer.GetImplementationFor<Helper.IDebugController>();
        protected static PoolingSystem.IPoolingManager poolingManager => DIContainer.GetImplementationFor<PoolingSystem.IPoolingManager>();
        protected static EntitySystem.IEntityControlSystem ECS => DIContainer.GetImplementationFor<EntitySystem.IEntityControlSystem>();
        protected static Data.IProfileHandler profileHandler => DIContainer.GetImplementationFor<Data.IProfileHandler>();
        protected static UI.IFloatingElementManager floatingManager => DIContainer.GetImplementationFor<UI.IFloatingElementManager>();
        protected static MessageSystem.IMessageHub messageHub => DIContainer.GetImplementationFor<MessageSystem.IMessageHub>();

        #region Behaviour 
        protected bool IsReady { get; private set; } = false;

        private void Start()
        {
            if (systemsInitializer.Finished)
            {
                StartInternal();
                return;
            }
            systemsInitializer.OnAllSystemsLoaded += StartInternal;
        }

        private void StartInternal() 
        {
            IsReady = true;
            systemsInitializer.OnAllSystemsLoaded -= StartInternal;
            OnSystemsInitialized();
        }

        /// <summary>
        /// Called after all systems have been initialized
        /// Should replace Start()
        /// </summary>
        protected virtual void OnSystemsInitialized() { }

        private void OnDestroy()
        {
            OnBeforeDestroy();
        }

        /// <summary>
        /// Called right after beeing destroyed
        /// </summary>
        protected virtual void OnBeforeDestroy() { }
        #endregion
    }
}