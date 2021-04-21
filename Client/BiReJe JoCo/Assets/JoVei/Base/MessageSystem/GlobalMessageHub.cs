using System.Collections;

namespace JoVei.Base.MessageSystem
{
    /// <summary>
    /// Implementation of BaseMessageHub as system
    /// </summary>
    public class GlobalMessageHub : BaseMessageHub, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IMessageHub>(this);
            yield return null;
        }

        public void CleanUp() { }

        protected override bool WriteLog { get { return globalVariables.GetVar<bool>("debug_mode"); } }
    }
}
