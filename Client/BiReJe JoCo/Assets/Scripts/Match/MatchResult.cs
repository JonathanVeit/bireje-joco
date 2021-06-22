namespace BiReJeJoCo.Backend
{
    public enum WinCondition 
    {
        None            = 0,
        CatchedHuned    = 1, 
        TimeOver        = 2,
        CrystalsCreated = 3,
    }

    [System.Serializable]
    public class MatchResult
    {
        public PlayerRole winner;
        public WinCondition condition;
        public string message;
    }
}