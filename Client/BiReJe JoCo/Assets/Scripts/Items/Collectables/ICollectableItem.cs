using JoVei.Base.Data;

namespace BiReJeJoCo.Items
{
    public interface ICollectableItem : IUniqueId
    {
        string InstanceId { get; }

        void InitializeCollectable(string instanceId);
        void OnCollect();
    }
}