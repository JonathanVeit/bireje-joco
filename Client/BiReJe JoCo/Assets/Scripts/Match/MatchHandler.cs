using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BiReJeJoCo
{
    public class MatchHandler : TickBehaviour
    {
        public MatchState State { get; protected set; }
        public MatchConfig MatchConfig { get; protected set; }

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            State = MatchState.InLobby;
            DontDestroyOnLoad(this);
            DIContainer.RegisterImplementation<MatchHandler>(this);
            messageHub.RegisterReceiver<OnLoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
        }

        protected virtual void OnLoadedLobbyScene(OnLoadedLobbySceneMsg msg)
        {
            LogMatchMessage("Loaded lobby scene");
            messageHub.UnregisterReceiver<OnLoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
            ConnectEvents();
        }

        protected virtual void ConnectEvents()
        {           
            photonMessageHub.RegisterReceiver<DefineMatchRulesPhoMsg>(this, OnDefineMatchRoles);
            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnStartMatch);
            photonMessageHub.RegisterReceiver<PausePausePhoMsg>(this, OnPauseMatch);
            photonMessageHub.RegisterReceiver<ContinueMatchPhoMsg>(this, OnContinueMatch);
            photonMessageHub.RegisterReceiver<EndMatchPhoMsg>(this, OnEndMatch);
            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);

            messageHub.RegisterReceiver<OnLoadedGameSceneMsg>(this, OnLoadedGameScene);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        protected virtual void DisconnectEvents()
        {
            Debug.Log(123);
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
            MatchConfig = castedMsg.config;

            localPlayer.SetRole(castedMsg.config.roles[localPlayer.NumberInRoom]);
            LogMatchMessage("Match rules synchronized");
        }

        protected virtual void OnStartMatch(PhotonMessage msg) 
        {
            State = MatchState.Running;
            LogMatchMessage("Match started");
        }

        protected virtual void OnPauseMatch(PhotonMessage msg)
        {
            State = MatchState.Paused;
            LogMatchMessage("Match paused");
        }

        protected virtual void OnContinueMatch(PhotonMessage msg)
        {
            State = MatchState.Running;
            LogMatchMessage("Match continued");
        }


        protected virtual void OnEndMatch(PhotonMessage msg)
        {
            LogMatchMessage("Match ended");
        }

        protected virtual void OnQuitMatch(PhotonMessage msg)
        {
            var casted = msg as QuitMatchPhoMsg;

            if (casted.leaveLobby)
            {
                DisconnectEvents();
                photonClient.LeaveLobby();
            }

            LogMatchMessage($"Match is quitted. Leave Lobby = {casted.leaveLobby}");
        }

        protected virtual void OnLeftLobby(OnLeftLobbyMsg msg)
        {
            DIContainer.UnregisterImplementation<MatchHandler>();
            messageHub.UnregisterReceiver(this);
            gameManager.OpenMainMenu();
            Destroy(this.gameObject);
        }
        #endregion

        #region Helper
        protected void LogMatchMessage(string message)
        {
            Debug.Log($"<color=green>[MatchHandler]</color> {message}");
        }
        #endregion
    }
}