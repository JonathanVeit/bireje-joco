using JoVei.Base;

namespace BiReJeJoCo
{
    /// <summary>
    /// Our system accessor with all additional systems that do not come from our code base
    /// </summary>
    public class SystemAccessor : BaseSystemAccessor
    {
        protected static Backend.PhotonConnectionWrapper photonConnectionWrapper => DIContainer.GetImplementationFor<Backend.PhotonConnectionWrapper>();
        protected static Backend.PhotonRoomWrapper photonRoomWrapper => DIContainer.GetImplementationFor<Backend.PhotonRoomWrapper>();
        protected static Backend.PhotonClient photonClient => DIContainer.GetImplementationFor<Backend.PhotonClient>();
        protected static Backend.PhotonMessageHub photonMessageHub => DIContainer.GetImplementationFor<Backend.PhotonMessageHub>();
        protected static Backend.PlayerManager playerManager => DIContainer.GetImplementationFor<Backend.PlayerManager>();
        protected static Backend.Player localPlayer => playerManager.LocalPlayer;
        protected static GameManager gameManager => DIContainer.GetImplementationFor<GameManager>();
    }
}