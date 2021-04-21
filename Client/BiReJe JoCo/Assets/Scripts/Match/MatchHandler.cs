using JoVei.Base;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo
{
    public class MatchHandler : TickBehaviour
    {
        public MatchState State { get; protected set; }

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            State = MatchState.WaitingForPlayer;

            DIContainer.RegisterImplementation<MatchHandler>(this);
            messageHub.ShoutMessage(this, new OnLoadedGameSceneMsg());
            ConnectEvents();
        }

        protected override void OnBeforeDestroy()
        {
            DIContainer.UnregisterImplementation<MatchHandler>();
        }

        private void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<StartGamePhoMsg>(this, OnStartGame);
            photonMessageHub.RegisterReceiver<PauseGamePhoMsg>(this, OnPauseGame);
            photonMessageHub.RegisterReceiver<ContinueGamePhoMsg>(this, OnContinuetGame);
        }
        #endregion

        #region Photon Messages
        protected virtual void OnStartGame(PhotonMessage msg) 
        {
            State = MatchState.Running;
        }

        protected virtual void OnPauseGame(PhotonMessage msg)
        {
            State = MatchState.Paused;
        }

        protected virtual void OnContinuetGame(PhotonMessage msg)
        {
            State = MatchState.Running;
        }
        #endregion
    }
}