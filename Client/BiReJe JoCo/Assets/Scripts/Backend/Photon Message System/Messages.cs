using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    #region Match Related 
    /// <summary>
    /// Fired by the host to inform each player that the match is beeing prepared (lobby -> game)
    /// </summary>
    public class PrepareMatchStartPhoMsg : PhotonMessage
    {
        public string matchScene;

        public PrepareMatchStartPhoMsg() { }

        public PrepareMatchStartPhoMsg(string matchScene)
        {
            this.matchScene = matchScene;
        }
    }

    /// <summary>
    /// Fired by the host to inform each player about the match rules, roles etc.
    /// </summary>
    public class DefineMatchRulesPhoMsg : PhotonMessage
    {
        public MatchConfig config;

        public DefineMatchRulesPhoMsg() { }

        public DefineMatchRulesPhoMsg(MatchConfig config)
        {
            this.config = config;
        }
    }

    /// <summary>
    /// Fired by the host to start the match 
    /// </summary>
    public class StartMatchPhoMsg : PhotonMessage
    {
        
    }

    /// <summary>
    /// Fired by the host to pause the match 
    /// </summary>
    public class PausePausePhoMsg : PhotonMessage
    {

    }

    /// <summary>
    /// Fired by the host to continue the match 
    /// </summary>
    public class ContinueMatchPhoMsg : PhotonMessage
    {

    }

    /// <summary>
    /// Fired by the host to end the match
    /// </summary>
    public class EndMatchPhoMsg : PhotonMessage
    {
        
    }

    /// <summary>
    /// Fired by the host to quit the match
    /// </summary>
    public class QuitMatchPhoMsg : PhotonMessage
    {
        public bool leaveLobby;

        public QuitMatchPhoMsg() { }
        public QuitMatchPhoMsg(bool leaveLobby)
        {
            this.leaveLobby = leaveLobby;
        }
    }
    #endregion

    public class OnSynchronizedTriggerPhoMsg : PhotonMessage
    {
        public int i;
        public int a;

        public OnSynchronizedTriggerPhoMsg() { }
        public OnSynchronizedTriggerPhoMsg(int id, int actor)
        {
            i = id;
            a = actor;
        }
    }
}
