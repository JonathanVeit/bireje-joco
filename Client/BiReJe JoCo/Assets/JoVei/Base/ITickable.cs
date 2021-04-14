namespace JoVei.Base
{
    /// <summary>
    /// Tickable with delta time
    /// </summary>
    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}