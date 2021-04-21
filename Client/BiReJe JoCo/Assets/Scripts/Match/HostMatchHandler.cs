using BiReJeJoCo.Backend;

namespace BiReJeJoCo
{
    public class HostMatchHandler : MatchHandler
    {
        private bool startedMatch = false;

        public override void Tick(float deltaTime)
        {
            if (State == MatchState.WaitingForPlayer) 
                WaitForPlayer();
        } 

        private void WaitForPlayer() 
        {
            if (!startedMatch && AllPlayerReady())
            {
                photonMessageHub.ShoutMessage(new StartGamePhoMsg(), PhotonMessageTarget.AllViaServer);
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
    }
}