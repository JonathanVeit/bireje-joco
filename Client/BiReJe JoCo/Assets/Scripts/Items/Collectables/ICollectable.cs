using JoVei.Base.Data;

namespace BiReJeJoCo.Items
{
    public interface ICollectable : IUniqueId
    {
        string InstanceId { get; }
        int SpawnPointIndex { get; }

        void InitializeCollectable(string instanceId, int spawnPointIndex);
        void OnCollect();
    }
}