namespace BiReJeJoCo.Backend
{
    public abstract class LocalTrigger : BaseTrigger
    {
        protected override abstract void OnTriggerInteracted(byte pointId);
    }
}
