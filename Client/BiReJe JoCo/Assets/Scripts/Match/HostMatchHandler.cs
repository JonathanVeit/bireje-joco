using BiReJeJoCo.Backend;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using BiReJeJoCo.Items;
using BiReJeJoCo.Character;

namespace BiReJeJoCo
{
    public class HostMatchHandler : MatchHandler
    {
        private bool startedMatch = false;

        private const int START_MATCH_DELAY = 5;

        #region Initialiation
        protected override void ConnectEvents()
        {
            base.ConnectEvents();
            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, OnPrepareMatchStart);
            photonMessageHub.RegisterReceiver<HuntedCatchedPhoMsg>(this, OnHuntedCatched);
            photonMessageHub.RegisterReceiver<HuntedFinishedObjectivePhoMsg>(this, OnHuntedFinishedObjective);
        }
        #endregion

        #region Lobby
        public void StartMatch(string matchMode)
        {
            photonMessageHub.ShoutMessage(new PrepareMatchStartPhoMsg(matchMode), PhotonMessageTarget.AllViaServer);
            lobbyManager.GetCurrentLobby().SetState(LobbyState.MatchRunning);
        }

        private void OnPrepareMatchStart(PhotonMessage msg)
        {
            var castedMsg = msg as PrepareMatchStartPhoMsg;
            var config = CreateMatchConfig(castedMsg.matchMode);

            photonMessageHub.ShoutMessage(new DefinedMatchRulesPhoMsg(config), PhotonMessageTarget.AllViaServer);
            LogMatchMessage("Match rules defined");
        }

        private MatchConfig CreateMatchConfig(string matchMode)
        {
            // match mode
            var mode = MatchModeMapping.GetMapping().GetElementForKey(matchMode);

            // roles 
            var allPlayer = playerManager.GetAllPlayer().ToList();

            // define hunted 
            Player hunted = GetRandomHunted(allPlayer);
            var hunter = new List<Player>(allPlayer);
            hunter.Remove(hunted);

            // save roles
            var playerRoles = new Dictionary<int, PlayerRole>();
            playerRoles.Add(hunted.NumberInRoom, PlayerRole.Hunted);

            for (int i = 0; i < hunter.Count; i++)
            {
                playerRoles.Add(hunter[i].NumberInRoom, PlayerRole.Hunter);
            }

            // spawn points
            var spawnPoints = new Dictionary<int, int>();
            var mapConfig = MapConfigMapping.GetMapping().GetElementForKey(mode.gameScene);

            // hunted 
            int huntedSpawnPoint = mapConfig.GetRandomHuntedSpawnPointIndex();
            spawnPoints.Add(hunted.NumberInRoom, huntedSpawnPoint);

            // hunter 
            var hunterSpawnPoints = mapConfig.GetRandomHunterSpawnPointIndeces(hunter.Count);
            for (int i = 0; i < hunter.Count; i++)
            {
                spawnPoints.Add(hunter[i].NumberInRoom, hunterSpawnPoints[i]);
            }

            // collectables
            var collectableConfigs = new List<CollectableSpawnConfig>();
            var collectableAmount = MatchModeMapping.GetMapping().GetElementForKey("default_match").huntedCollectables;
            var collectableSpawnPoints = mapConfig.GetRandomCollectableSpawnPointIndices(collectableAmount, true, mode.minCollectableDistance);
            for (int i = 0; i < collectableAmount; i++)
            {
                var spawnConfig = new CollectableSpawnConfig()
                {
                    i = "collectable_coral_ammo",
                    s = collectableSpawnPoints[i]
                };

                collectableConfigs.Add(spawnConfig);
            }

            // create match config 
            var config = new MatchConfig()
            {
                roles = playerRoles,
                spawnPos = spawnPoints,
                matchMode = matchMode,

                collectables = collectableConfigs,
                collectableSeed = DateTime.Now.Second,
            };

            if (globalVariables.HasVar("force_hunter") && 
                globalVariables.GetVar<bool>("force_hunter"))
            {
                config.roles[localPlayer.NumberInRoom] = PlayerRole.Hunter;
                config.spawnPos[localPlayer.NumberInRoom] = 0;
            }

            return config;
        }

        private static Player GetRandomHunted(List<Player> allPlayer)
        {
            var preferedHunted = playerManager.GetAllPlayer(x => x.PreferedRole == PlayerRole.Hunted || x.PreferedRole == PlayerRole.None);
            if (preferedHunted.Length == 0)
                return allPlayer[UnityEngine.Random.Range(0, allPlayer.Count)];
            return preferedHunted[UnityEngine.Random.Range(0, preferedHunted.Length)];
        }

        protected override void OnDefineMatchRules(PhotonMessage msg)
        {
            var castedMsg = msg as DefinedMatchRulesPhoMsg;
            base.OnDefineMatchRules(msg);

            photonRoomWrapper.LoadLevel(castedMsg.config.matchScene);
        }
        #endregion

        #region Duration
        protected override IEnumerator DurationCounter(DateTime startDate, DateTime endDate)
        {
            yield return base.DurationCounter(startDate, endDate);

            var hunterCharacter = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted)[0].PlayerCharacter;
            var totalCorals = hunterCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().CoralMechanic.CalculateTotalCorals();

            MatchResult result;
            if (totalCorals >= MatchConfig.Mode.coralsToWin)
            {
                result = new MatchResult()
                {
                    winner = PlayerRole.Hunted,
                    condition = WinCondition.TimeOver,
                    message = $"Time is over! The monster placed {totalCorals} spores and won.",
                };
            }
            else
            {
                result = new MatchResult()
                {
                    winner = PlayerRole.Hunted,
                    condition = WinCondition.TimeOver,
                    message = $"Time is over! The monster only created {totalCorals} spores and lost.",
                };
            }

            photonMessageHub.ShoutMessage(new FinishMatchPhoMsg() { result = result }, PhotonMessageTarget.AllViaServer);
        }
        #endregion

        #region Match
        protected override void OnLoadedGameScene(LoadedGameSceneMsg msg)
        {
            base.OnLoadedGameScene(msg);
            startedMatch = false;
        }

        public override void Tick(float deltaTime)
        {
            if (State == MatchState.WaitingForPlayer) 
                WaitForPlayer();
        } 
        private void WaitForPlayer() 
        {
            if (!startedMatch && AllPlayerReady())
            {
                var startDate = DateTime.UtcNow.AddSeconds(START_MATCH_DELAY);
                var endDate = DateTime.UtcNow.AddSeconds((MatchConfig.Mode.duration * 60) + START_MATCH_DELAY);

                photonMessageHub.ShoutMessage(new StartMatchPhoMsg(startDate, endDate), PhotonMessageTarget.AllViaServer);
                startedMatch = true;
            }
        }
        private bool AllPlayerReady() 
        {
            foreach (var curPlayer in playerManager.GetAllPlayer())
            {
                if (curPlayer.State != PlayerState.Ready)
                    return false;
            }

            return true;
        }


        public void RestartMatch(string match_mode)
        {
            System.Action<PhotonMessage> callback = (x) =>
            {
                LogMatchMessage("Restart match");
                StartMatch(match_mode);
            };
            callback += (x) => { photonMessageHub.UnregisterReceiver<CloseMatchPhoMsg>(this, callback); };
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, callback);

            photonMessageHub.ShoutMessage(new CloseMatchPhoMsg(CloseMatchMode.LoadLevel), PhotonMessageTarget.AllViaServer);
        }
        public void CloseMatch(CloseMatchMode mode)
        {
            photonMessageHub.ShoutMessage<CloseMatchPhoMsg>(PhotonMessageTarget.AllViaServer, mode);
        }
        
        private void OnHuntedCatched(PhotonMessage msg)
        {
            var result = new MatchResult()
            {
                winner = PlayerRole.Hunter,
                condition = WinCondition.CatchedHuned,
                message = "Monster has been catched!",
            };

            photonMessageHub.ShoutMessage<FinishMatchPhoMsg>(PhotonMessageTarget.AllViaServer, result);
        }
        private void OnHuntedFinishedObjective(PhotonMessage msg)
        {
            var result = new MatchResult()
            {
                winner = PlayerRole.Hunted,
                condition = WinCondition.CoralsCreated,
                message = $"The monster created {MatchConfig.Mode.maxCorals} spores.",
            };

            photonMessageHub.ShoutMessage<FinishMatchPhoMsg>(PhotonMessageTarget.AllViaServer, result);
        }

        protected override void OnMatchClosed(PhotonMessage msg)
        {
            var casted = msg as CloseMatchPhoMsg;

            // needs to be destroyed over photon 
            localPlayer.DestroyPlayerCharacter();
            StopCoroutine(durationCounter);

            if (casted.mode == CloseMatchMode.LeaveLobby)
            {
                photonMessageHub.UnregisterReceiver(this);
                photonClient.LeaveLobby();
            }
            else if (casted.mode == CloseMatchMode.ReturnToLobby)
            {
                // delay the loading so player characters can be destroyed by photon
                StartCoroutine(LoadLobbyAsync());
            }

            LogMatchMessage($"Match is closed. Mode = {casted.mode}");
        }
        private IEnumerator LoadLobbyAsync() 
        {
            yield return new WaitForSeconds(0.5f);
            lobbyManager.GetCurrentLobby().SetState(LobbyState.Open);
            photonRoomWrapper.LoadLevel("lobby_scene");
        }
        #endregion
    }
}