using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;

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

        private void ConnectEvents()
        {
            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnStartGame);
            photonMessageHub.RegisterReceiver<PausePausePhoMsg>(this, OnPauseGame);
            photonMessageHub.RegisterReceiver<ContinueMatchPhoMsg>(this, OnContinuetGame);
            photonMessageHub.RegisterReceiver<EndMatchPhoMsg>(this, OnEndMatch);
            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);

            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        private void DisonnectEvents()
        {
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        protected virtual void OnStartGame(PhotonMessage msg) 
        {
            State = MatchState.Running;
            Debug.Log("Match started");
        }

        protected virtual void OnPauseGame(PhotonMessage msg)
        {
            State = MatchState.Paused;
            Debug.Log("Match paused");
        }

        protected virtual void OnContinuetGame(PhotonMessage msg)
        {
            State = MatchState.Running;
            Debug.Log("Match continued");
        }


        private void OnEndMatch(PhotonMessage msg)
        {
            Debug.Log("Match ended");
        }

        private void OnQuitMatch(PhotonMessage msg)
        {
            var casted = msg as QuitMatchPhoMsg;

            DisonnectEvents();
            DIContainer.UnregisterImplementation<MatchHandler>();
            if (casted.leaveLobby) photonClient.LeaveLobby();
            Debug.Log($"Match is quitted. Leave Lobby = {casted.leaveLobby}");
        }

        private void OnLeftLobby(OnLeftLobbyMsg msg)
        {
            messageHub.UnregisterReceiver(this);
            gameManager.OpenMainMenu();
        }
        #endregion
    }
}