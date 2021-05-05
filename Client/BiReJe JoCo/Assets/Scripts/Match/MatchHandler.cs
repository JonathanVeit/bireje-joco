using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;

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
            DontDestroyOnLoad(this);
            State = MatchState.InLobby;
            
            DIContainer.RegisterImplementation<MatchHandler>(this);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
        }

        protected virtual void OnLoadedLobbyScene(LoadedLobbySceneMsg msg)
        {
            LogMatchMessage("Loaded lobby scene");
            messageHub.UnregisterReceiver<LoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
            ConnectEvents();
        }

        protected virtual void ConnectEvents()
        {           
            photonMessageHub.RegisterReceiver<DefinedMatchRulesPhoMsg>(this, OnDefineMatchRules);
            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, OnStartMatch);
            photonMessageHub.RegisterReceiver<PauseMatchPhoMsg>(this, OnPauseMatch);
            photonMessageHub.RegisterReceiver<ContinueMatchPhoMsg>(this, OnContinueMatch);
            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnMatchFinished);
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);

            messageHub.RegisterReceiver<LoadedGameSceneMsg>(this, OnLoadedGameScene);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
        }
        #endregion

        #region Events
        protected virtual void OnDefineMatchRules(PhotonMessage msg)
        {
            var castedMsg = msg as DefinedMatchRulesPhoMsg;
            MatchConfig = castedMsg.config;

            localPlayer.SetRole(castedMsg.config.roles[localPlayer.NumberInRoom]);
            LogMatchMessage("Match rules synchronized");
        }

        protected virtual void OnLoadedGameScene(LoadedGameSceneMsg msg) 
        {
            State = MatchState.WaitingForPlayer;
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


        protected virtual void OnMatchFinished(PhotonMessage msg)
        {
            LogMatchMessage("Match ended");
            State = MatchState.Result;
        }
        protected virtual void OnMatchClosed(PhotonMessage msg)
        {
            var casted = msg as CloseMatchPhoMsg;
            if (casted.mode == CloseMatchMode.LeaveLobby)
            {
                photonMessageHub.UnregisterReceiver(this);
                photonClient.LeaveLobby();
            }

            LogMatchMessage($"Match is closed. Mode = {casted.mode}");
        }
        protected virtual void OnLeftLobby(LeftLobbyMsg msg)
        {
            DIContainer.UnregisterImplementation<MatchHandler>();
            messageHub.UnregisterReceiver(this);
            gameManager.OpenMainMenu();
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