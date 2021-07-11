using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;
using System.Collections;
using BiReJeJoCo.UI;
using System;

namespace BiReJeJoCo
{
    public class MatchHandler : TickBehaviour
    {
        public MatchState State { get; protected set; }
        public MatchConfig MatchConfig { get; protected set; }

        protected Coroutine durationCounter;
        private int lastTimeMessage;


        #region Initialization
        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            DontDestroyOnLoad(this);
            
            DIContainer.RegisterImplementation<MatchHandler>(this);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
            DIContainer.UnregisterImplementation<MatchHandler>();
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

        protected virtual void DisconnectEvents() 
        {
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Duration
        protected virtual IEnumerator DurationCounter(DateTime startDate, DateTime endDate)
        {
            var waiter = new WaitForEndOfFrame();

            while (startDate > DateTime.UtcNow) 
            {
                var seconds = (int) (startDate - DateTime.UtcNow).TotalSeconds;
                uiManager.GetInstanceOf<GameUI>().UpdateStartCountdown(seconds);
                yield return waiter;
            }
            uiManager.GetInstanceOf<GameUI>().CloseCountdownOverlay();
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));
            State = MatchState.Running;

            while (endDate > DateTime.UtcNow)
            {
                var seconds = (int)(endDate - DateTime.UtcNow).TotalSeconds;
                uiManager.GetInstanceOf<GameUI>().UpdateMatchDuration(ConvertSecondsToTimeString(seconds));
                
                if (seconds == 60 ||  seconds == 30)
                {
                    if (seconds != lastTimeMessage)
                    {
                        uiManager.GetInstanceOf<GameUI>().ShowMessage($"{seconds} seconds!", 4, Color.white);
                        lastTimeMessage = seconds;
                    }
                }

                yield return waiter;
            }

            uiManager.GetInstanceOf<GameUI>().UpdateMatchDuration(ConvertSecondsToTimeString(0));
        }
        protected string ConvertSecondsToTimeString(int seconds)
        {
            var m = Mathf.FloorToInt(seconds / 60);
            var s = seconds - (m * 60);

            var m_str = m > 9 ? m.ToString() : "0" + m.ToString();
            var s_str = s > 9 ? s.ToString() : "0" + s.ToString();

            return string.Format("{0}:{1}", m_str, s_str);
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

            foreach (var curConfig in MatchConfig.collectables)
                collectablesManager.CreateCollectable(curConfig);

            LogMatchMessage("Spawned collectables");
        }
        protected virtual void OnStartMatch(PhotonMessage msg) 
        {
            var castedMsg = msg as StartMatchPhoMsg;

            State = MatchState.CountDown;
            uiManager.GetInstanceOf<GameUI>().CloseLoadingOverlay();
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Loading));

            durationCounter = StartCoroutine(DurationCounter(castedMsg.startDate, castedMsg.endDate));
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
            StopCoroutine(durationCounter);
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
                State = MatchState.InLobby;
            }

            // needs to be destroyed over photon 
            photonRoomWrapper.Destroy(localPlayer.PlayerCharacter.gameObject);
            StopCoroutine(durationCounter);
            State = MatchState.None;

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