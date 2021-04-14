namespace JoVei.Base.TickSystem
{
    /// <summary>
    /// Interface for a ticksystem devided into Update, Fixed and Late
    /// </summary>
    public interface ITickSystem
    {
        float TimeScale { get; set; } // overall timescale 

        void UpdateSystem(); // default unity update 
        void FixedUpdateSystem(); // default fixed unity update 
        void LateUpdateSystem(); // default late update 

        void Register(ITickable tickable); // default update 
        void Register(ITickable tickable, string tickRegion); // region 
        void Unregister(ITickable tickable); // unregister from its region

        public void RegisterRegion(string id, ITickRegionConfig regionConfig); // register with config 
        public void RegisterRegion(string id, TickUpdateType updateType, float border, float scale); // register without a config 
        public void UnregisterRegion(string id); // unregister specific
    }
}
