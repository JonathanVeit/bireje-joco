using BiReJeJoCo.Backend;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using BiReJeJoCo.UI;

namespace BiReJeJoCo
{
    public class HostMatchHandler : MatchHandler
    {
        private bool startedMatch = false;
        public int MatchDuration { get; protected set; } = 10 * 60;

        #region Initialiation
        protected override void ConnectEvents()
        {
            base.ConnectEvents();
            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, OnPrepareMatchStart);
            photonMessageHub.RegisterReceiver<HuntedKilledPhoMsg>(this, OnHuntedKilled);
        }
        #endregion

        #region Lobby
        public void StartMatch(string match_scene)
        {
            photonMessageHub.ShoutMessage(new PrepareMatchStartPhoMsg(match_scene), PhotonMessageTarget.AllViaServer);
        }

        private void OnPrepareMatchStart(PhotonMessage msg)
        {
            var castedMsg = msg as PrepareMatchStartPhoMsg;
            var config = CreateMatchConfig(castedMsg.matchScene);

            photonMessageHub.ShoutMessage(new DefinedMatchRulesPhoMsg(config), PhotonMessageTarget.AllViaServer);
            LogMatchMessage("Match rules defined");
        }

        private MatchConfig CreateMatchConfig(string matchScene)
        {
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
            var mapConfig = MapConfigMapping.GetMapping().GetElementForKey(matchScene);

            // hunted 
            int huntedSpawnPoint = mapConfig.GetRandomHuntedSpawnPointIndex();
            spawnPoints.Add(hunted.NumberInRoom, huntedSpawnPoint);

            // hunter 
            var hunterSpawnPoints = mapConfig.GetRandomHunterSpawnPointIndex(hunter.Count);
            for (int i = 0; i < hunter.Count; i++)
            {
                spawnPoints.Add(hunter[i].NumberInRoom, hunterSpawnPoints[i]);
            }

            // create match config 
            var config = new MatchConfig()
            {
                matchScene = matchScene,
                roles = playerRoles,
                spawnPos = spawnPoints,
                duration = MatchDuration,
            };

            return config;
        }

        private static Player GetRandomHunted(List<Player> allPlayer)
        {
            var preferedHunted = playerManager.GetAllPlayer(x => x.PreferedRole == PlayerRole.Hunted);
            if (preferedHunted.Length == 0)
                return allPlayer[Random.Range(0, allPlayer.Count)];
            return preferedHunted[Random.Range(0, preferedHunted.Length)];
        }

        protected override void OnDefineMatchRules(PhotonMessage msg)
        {
            var castedMsg = msg as DefinedMatchRulesPhoMsg;
            base.OnDefineMatchRules(msg);

            photonRoomWrapper.LoadLevel(castedMsg.config.matchScene);
        }
        #endregion

        #region Duration
        protected override IEnumerator DurationCounter(int duration)
        {
            var waiter = new WaitForSeconds(1);

            for (int i = duration; i >= 0; i--)
            {
                uiManager.GetInstanceOf<GameUI>().UpdateMatchDuration(ConvertSecondsToTimeString(i));
                yield return waiter;
            }

            uiManager.GetInstanceOf<GameUI>().UpdateMatchDuration("");

            var result = new MatchResult()
            {
                winner = PlayerRole.Hunted,
                message = "Time is over!",
            };
            photonMessageHub.ShoutMessage(new FinishMatchPhoMsg() { result = result }, PhotonMessageTarget.AllViaServer);
        }

        public void SetDuration(int duration) 
        {
            MatchDuration = duration;
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
                photonMessageHub.ShoutMessage(new StartMatchPhoMsg(), PhotonMessageTarget.AllViaServer);
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


        public void RestartMatch(string scene_name)
        {
            System.Action<PhotonMessage> callback = (x) =>
            {
                LogMatchMessage("Restart match");
                StartMatch(scene_name);
            };
            callback += (x) => { photonMessageHub.UnregisterReceiver<CloseMatchPhoMsg>(this, callback); };
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, callback);

            photonMessageHub.ShoutMessage(new CloseMatchPhoMsg(CloseMatchMode.LoadLevel), PhotonMessageTarget.AllViaServer);
        }
        public void CloseMatch(CloseMatchMode mode)
        {
            photonMessageHub.ShoutMessage<CloseMatchPhoMsg>(PhotonMessageTarget.AllViaServer, mode);
        }
        
        private void OnHuntedKilled(PhotonMessage msg)
        {
            var result = new MatchResult()
            {
                winner = PlayerRole.Hunter,
                message = "Monster has been killed!",
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
            photonRoomWrapper.LoadLevel("lobby_scene");
        }
        #endregion
    }
}