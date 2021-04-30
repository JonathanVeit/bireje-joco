using BiReJeJoCo.Backend;
using System.Collections.Generic;
using System.Linq;

namespace BiReJeJoCo
{
    public class HostMatchHandler : MatchHandler
    {
        private bool startedMatch = false;

        #region Initialiation
        protected override void ConnectEvents()
        {
            base.ConnectEvents();
            photonMessageHub.RegisterReceiver<PrepareMatchStartPhoMsg>(this, OnPrepareMatchStart);
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

            photonMessageHub.ShoutMessage(new DefineMatchRulesPhoMsg(config), PhotonMessageTarget.AllViaServer);
            LogMatchMessage("Match rules defined");
        }

        private MatchConfig CreateMatchConfig(string matchScene) 
        {
            // roles 
            var allPlayer = playerManager.GetAllPlayer().ToList();
            
            // define hunted 
            var hunted = allPlayer[UnityEngine.Random.Range(0, allPlayer.Count)];
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
            };

            return config;
        }

        protected override void OnDefineMatchRoles(PhotonMessage msg)
        {
            var castedMsg = msg as DefineMatchRulesPhoMsg;
            base.OnDefineMatchRoles(msg);

            LoadLevel(castedMsg.config.matchScene);
        }
        #endregion

        #region Match
        protected override void OnLoadedGameScene(OnLoadedGameSceneMsg msg)
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
            callback += (x) => { photonMessageHub.UnregisterReceiver<QuitMatchPhoMsg>(this, callback); };
            photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, callback);

            photonMessageHub.ShoutMessage(new QuitMatchPhoMsg(false), PhotonMessageTarget.AllViaServer);
        }
        #endregion

        #region Helper
        private void LoadLevel(string level_name)
        {
            photonRoomWrapper.LoadLevel(level_name);
        }
        #endregion
    }
}