using JoVei.Base;
using BiReJeJoCo.Backend;
using UnityEngine;
using System.Collections;
using BiReJeJoCo.UI;

namespace BiReJeJoCo
{
    public class MatchHandler : TickBehaviour
    {
        public MatchState State { get; protected set; }
        public MatchConfig MatchConfig { get; protected set; }

        protected Coroutine durationCounter;

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

        #region Duration
        protected virtual IEnumerator DurationCounter(int duration)
        {
            var waiter = new WaitForSeconds(1);

            for (int i = duration; i >= 0; i--)
            {
                uiManager.GetInstanceOf<GameUI>().UpdateDuration(ConvertSecondsToTimeString(i));
                yield return waiter;
            }

            uiManager.GetInstanceOf<GameUI>().UpdateDuration("");
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
        }
        protected virtual void OnStartMatch(PhotonMessage msg) 
        {
            State = MatchState.Running;
            durationCounter = StartCoroutine(DurationCounter(MatchConfig.duration));
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
            }

            // needs to be destroyed over photon 
            photonRoomWrapper.Destroy(localPlayer.PlayerCharacter.gameObject);
            StopCoroutine(durationCounter);

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