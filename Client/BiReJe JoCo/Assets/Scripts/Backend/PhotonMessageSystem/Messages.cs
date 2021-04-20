namespace BiReJeJoCo.Backend
{
    /// <summary>
    /// Fired when a player successfully loaded the game scene
    /// </summary>
    public class OnPlayerLoadedGameMsg : PhotonMessage
    {
        public string playerId { get; private set; }

        public OnPlayerLoadedGameMsg(string playerId)
        {
            this.playerId = playerId;    
        }
    }


    public class StartGameMsg : PhotonMessage
    {
        
    }

    public class PauseGameMsg : PhotonMessage
    {

    }
}
