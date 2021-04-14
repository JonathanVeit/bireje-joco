namespace BiReJeJoCo
{    
    /// <summary>
    /// System behaviour with automated registration at tick system with default region
    /// </summary>
    public class TickBehaviour : SystemBehaviour, JoVei.Base.ITickable
    {
        protected override void OnSystemsInitialized()
        {
            tickSystem.Register(this);
        }

        public virtual void Tick(float deltaTime) { }

        protected override void OnBeforeDestroy()
        {
            tickSystem.Unregister(this);
        }
    }
}