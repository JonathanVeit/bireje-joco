﻿namespace BiReJeJoCo.Backend
{
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
}
