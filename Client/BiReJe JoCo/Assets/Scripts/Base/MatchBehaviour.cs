//using BiReJeJoCo.Backend;

//namespace BiReJeJoCo
//{
//    public class MatchBehaviour : SystemBehaviour
//    {
//        protected sealed override void OnSystemsInitialized()
//        {
//            ConnectEvents();
//            OnInitialized();
//        }

//        protected sealed override void OnBeforeDestroy()
//        {
//            messageHub.UnregisterReceiver(this);
//            OnDestroyed();
//        }

//        private void ConnectEvents()
//        {
//            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, HandleLobbySceneLoaded);
//            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, HandlePrepareMatchStart);
//            photonMessageHub.RegisterReceiver<DefineMatchRulesPhoMsg>(this, HandleMatchRulesDegined);
//            photonMessageHub.RegisterReceiver<StartMatchPhoMsg>(this, HandleStartMatch);
//            photonMessageHub.RegisterReceiver<PauseMatchPhoMsg>(this, HandlePauseMatch);
//            photonMessageHub.RegisterReceiver<ContinueMatchPhoMsg>(this, HandleContinueMatch);
//            photonMessageHub.RegisterReceiver<EndMatchPhoMsg>(this, HandleEndMatch);
//            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);
//        }
//        private void DisconnectEvents()
//        {
//            photonMessageHub.UnregisterReceiver(this);
//        }

//        #region Events
//        private void HandleLobbySceneLoaded(LoadedLobbySceneMsg msg)
//        {
//            OnLobbySceneLoaded();
//        }

//        private void HandlePrepareMatchStart(PhotonMessage msg)
//        {
//            OnPrepareMatchStart();
//        }

//        private void HandleMatchRulesDegined(PhotonMessage msg)
//        {
//            OnMatchRulesDefined();
//        }

//        private void HandleStartMatch(PhotonMessage msg)
//        {
//            OnMatchStarted();
//        }

//        private void HandlePauseMatch(PhotonMessage msg)
//        {
//            OnMatchPaused();
//        }

//        private void HandleContinueMatch(PhotonMessage msg)
//        {
//            OnMatchContinued();
//        }

//        private void HandleEndMatch(PhotonMessage msg)
//        {
//            OnMatchEnded();
//        }

//        private void OnQuitMatch(PhotonMessage msg)
//        {
//            var casted = msg as QuitMatchPhoMsg;
//            DisconnectEvents();
//            OnMatchQuit(casted.leaveLobby);
//        }
//        #endregion

//        #region Virtual Member
//        protected virtual void OnInitialized() { }

//        protected virtual void OnDestroyed() { }


//        protected virtual void OnLobbySceneLoaded() { }
//        protected virtual void OnPrepareMatchStart() { }
//        protected virtual void OnMatchRulesDefined() { }
//        protected virtual void OnGameSceneLoaded() { }

//        protected virtual void OnMatchStarted() { }
//        protected virtual void OnMatchPaused() { }
//        protected virtual void OnMatchContinued() { }
//        protected virtual void OnMatchEnded() { }
//        protected virtual void OnMatchQuit(bool leaveLobby) { }
//        #endregion
//    }
//}
