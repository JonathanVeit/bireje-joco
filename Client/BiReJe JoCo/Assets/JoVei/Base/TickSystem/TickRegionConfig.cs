namespace JoVei.Base.TickSystem
{
    /// <summary>
    /// Default implementation of ITickRegionConfig 
    /// </summary>
    public class TickRegionConfig : ITickRegionConfig
    {
        public TickUpdateType UpdateType { get; private set; }
        public float Border { get; private set; }
        public float Scale { get; private set; }

        public TickRegionConfig(TickUpdateType type, float border, float scale)
        {
            UpdateType = type;
            Border = border;
            Scale = scale;
        }
    }
}
