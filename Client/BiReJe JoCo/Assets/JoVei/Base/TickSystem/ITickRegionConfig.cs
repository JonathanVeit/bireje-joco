namespace JoVei.Base.TickSystem
{
    /// <summary>
    /// Interface for configuration of a tick region with update type, border and scale 
    /// </summary>
    public interface ITickRegionConfig
    {
        public TickUpdateType UpdateType { get; }
        public float Border { get; }
        public float Scale { get; }
    }
}
