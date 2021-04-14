namespace JoVei.Base.Data
{
    /// <summary>
    /// Instance with a level, unique Id and an instance Id
    /// </summary>
    public interface IInstance
    {
        string InstanceId { get; }

        void Create(string instanceId);
    }
}