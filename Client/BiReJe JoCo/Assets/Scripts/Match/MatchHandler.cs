using JoVei.Base;

namespace BiReJeJoCo
{
    public class MatchHandler : SystemBehaviour
    {
        protected override void OnSystemsInitialized()
        {
            DIContainer.RegisterImplementation<MatchHandler>(this);

            messageHub.ShoutMessage(this, new OnLoadedGameMsg());
        }
    }
}