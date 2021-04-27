using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BiReJeJoCo
{
    public class MatchHandler : TickBehaviour
    {
        public MatchState State { get; protected set; }

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            State = MatchState.InLobby;
            DIContainer.RegisterImplementation<MatchHandler>(this);
            messageHub.RegisterReceiver<OnLoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
        }

        protected virtual void OnLoadedLobbyScene(OnLoadedLobbySceneMsg msg)
        {
            messageHub.UnregisterReceiver<OnLoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
            ConnectEvents();
        }

        protected virtual void ConnectEvents()
        {           
            photonMessageHub.RegisterReceiver<DefineMatchRulesPhoMsg>(this, OnDefineMatchRoles);
            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnStartGame);
            photonMessageHub.RegisterReceiver<PausePausePhoMsg>(this, OnPauseGame);
            photonMessageHub.RegisterReceiver<ContinueMatchPhoMsg>(this, OnContinuetGame);
            photonMessageHub.RegisterReceiver<EndMatchPhoMsg>(this, OnEndMatch);
            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);

            messageHub.RegisterReceiver<OnLoadedGameSceneMsg>(this, OnLoadedGameScene);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        protected virtual void DisconnectEvents()
        {
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        protected virtual void OnLoadedGameScene(OnLoadedGameSceneMsg msg) 
        {
            State = MatchState.WaitingForPlayer;
        }

        protected virtual void OnDefineMatchRoles(PhotonMessage msg)
        {
            var castedMsg = msg as DefineMatchRulesPhoMsg;
            localPlayer.SetRole(castedMsg.roles[localPlayer.NumberInRoom]);
            Debug.Log("Match rules defined");
        }

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

            DisconnectEvents();
            if (casted.leaveLobby) photonClient.LeaveLobby();
            Debug.Log($"Match is quitted. Leave Lobby = {casted.leaveLobby}");
        }

        private void OnLeftLobby(OnLeftLobbyMsg msg)
        {
            DIContainer.UnregisterImplementation<MatchHandler>();
            messageHub.UnregisterReceiver(this);
            gameManager.OpenMainMenu();
            Destroy(this.gameObject);
        }
        #endregion
    }
}