using BiReJeJoCo.Backend;
using System.Collections.Generic;

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
        public void StartMatch() 
        {
            photonMessageHub.ShoutMessage(new PrepareMatchStartPhoMsg(), PhotonMessageTarget.AllViaServer);
        }

        private void OnPrepareMatchStart(PhotonMessage msg)
        {
            var allPlayer = playerManager.GetAllPlayer();
            var huntedIndex = UnityEngine.Random.Range(0, allPlayer.Length);

            var playerRoles = new Dictionary<int, PlayerRole>();
            for (int i = 0; i < allPlayer.Length; i++)
            {
                playerRoles.Add(allPlayer[i].NumberInRoom, i == huntedIndex ? PlayerRole.Hunted : PlayerRole.Hunter);
            }

            photonMessageHub.ShoutMessage(new DefineMatchRulesPhoMsg(playerRoles), PhotonMessageTarget.AllViaServer);
        }

        protected override void OnDefineMatchRoles(PhotonMessage msg)
        {
            base.OnDefineMatchRoles(msg);
            photonRoomWrapper.LoadLevel("game_scene");
        }
        #endregion

        #region Match
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
        #endregion

    }
}