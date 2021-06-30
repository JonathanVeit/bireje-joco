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
        protected static Backend.LocalPlayer localPlayer => playerManager.LocalPlayer;
        protected static Backend.LobbyManager lobbyManager => DIContainer.GetImplementationFor<Backend.LobbyManager>();
        protected static Backend.SyncVarHub syncVarHub => DIContainer.GetImplementationFor<Backend.SyncVarHub>();

        protected static GameManager gameManager => DIContainer.GetImplementationFor<GameManager>();
        protected static UI.UIManager uiManager => DIContainer.GetImplementationFor<UI.UIManager>();
        protected static MatchHandler matchHandler => DIContainer.GetImplementationFor<MatchHandler>();
        protected static Items.CollectablesManager collectablesManager => DIContainer.GetImplementationFor<Items.CollectablesManager>();

        protected static Sound.SoundEffectManager soundEffectManager => DIContainer.GetImplementationFor<Sound.SoundEffectManager>();
    }
}