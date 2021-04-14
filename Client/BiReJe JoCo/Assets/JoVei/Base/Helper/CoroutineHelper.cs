namespace JoVei.Base.Helper
{
    /// <summary>
    /// Allows to run coroutines from non-monobehaviours
    /// </summary>
    public class CoroutineHelper : BaseSystemSingleton<CoroutineHelper>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
    }
}