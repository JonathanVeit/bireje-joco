using Newtonsoft.Json;
using System;

namespace BiReJeJoCo.Backend
{
    /// <summary>
    /// Fired by the host to inform each player that the match is beeing prepared (lobby -> game)
    /// </summary>
    public class PrepareMatchStartPhoMsg : PhotonMessage
    {
        public string matchMode;

        public PrepareMatchStartPhoMsg() { }

        public PrepareMatchStartPhoMsg(string matchScene)
        {
            this.matchMode = matchScene;
        }
    }

    /// <summary>
    /// Fired by the host to inform each player about the match rules, roles etc.
    /// </summary>
    public class DefinedMatchRulesPhoMsg : PhotonMessage
    {
        public MatchConfig config;

        public DefinedMatchRulesPhoMsg() { }

        public DefinedMatchRulesPhoMsg(MatchConfig config)
        {
            this.config = config;
        }
    }

    /// <summary>
    /// Fired by the host to start the match 
    /// </summary>
    public class StartMatchPhoMsg : PhotonMessage
    {
        public DateTime startDate;
        public DateTime endDate;

        public StartMatchPhoMsg() { }
        public StartMatchPhoMsg(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }

    /// <summary>
    /// Fired by the host to pause the match 
    /// </summary>
    public class PauseMatchPhoMsg : PhotonMessage
    {

    }

    /// <summary>
    /// Fired by the host to continue the match 
    /// </summary>
    public class ContinueMatchPhoMsg : PhotonMessage
    {

    }

    /// <summary>
    /// Fired by the host to end the match with a certain result
    /// </summary>
    public class FinishMatchPhoMsg : PhotonMessage
    {
        public MatchResult result;

        public FinishMatchPhoMsg() { }
        public FinishMatchPhoMsg(MatchResult result)
        {
            this.result = result;
        }
    }

    /// <summary>
    /// Defines how a match will be closed
    /// </summary>
    public enum CloseMatchMode
    {
        None    = 0,
        LeaveLobby = 1,
        ReturnToLobby  = 2,
        LoadLevel   = 3,
    }

    /// <summary>
    /// Fired by the host to close the match (rehost possible)
    /// </summary>
    public class CloseMatchPhoMsg : PhotonMessage
    {
        public CloseMatchMode mode;

        public CloseMatchPhoMsg() { }
        public CloseMatchPhoMsg(CloseMatchMode mode)
        {
            this.mode = mode;
        }
    }

    /// <summary>
    /// Fired by a hunter when hitting the hunted with a bullet
    /// </summary>
    public class HuntedHitByBulletPhoMsg : PhotonMessage
    {
        public float dmg;

        public HuntedHitByBulletPhoMsg() { }
        public HuntedHitByBulletPhoMsg(float damage) 
        {
            dmg = damage;
        }
    }

    /// <summary>
    /// Fired by the hunted when beeing killed
    /// </summary>
    public class HuntedCatchedPhoMsg : PhotonMessage
    { 
    }

    /// <summary>
    /// Called by any player when interacting with a trigger
    /// </summary>
    public class TriggerPointInteractedPhoMsg : PhotonMessage
    {
        public byte i;
        public byte ti;
        public int a;

        public TriggerPointInteractedPhoMsg() { }
        public TriggerPointInteractedPhoMsg(byte id, byte pointId, int actor)
        {
            i = id;
            ti = pointId;
            a = actor;
        }
    }

    /// <summary>
    /// Called by any player when collecting an item
    /// </summary>
    public class CollectItemPhoMsg : PhotonMessage
    {
        [JsonIgnore]
        public string InstanceId => i;
        [JsonIgnore]
        public int playerNumber => n;

        public string i; // instance id
        public int n; // player number

        public CollectItemPhoMsg() { }
        public CollectItemPhoMsg(string instanceId, int playerNumber)
        {
            i = instanceId;
            n = playerNumber;
        }
    }

    /// <summary>
    /// Called by the hunted to spawn new ammo
    /// </summary>
    public class SpawnNewCoralAmmoPhoMsg : PhotonMessage
    {
        public int pointIndex;

        public SpawnNewCoralAmmoPhoMsg() { }
        public SpawnNewCoralAmmoPhoMsg(int pointIndex)
        {
            this.pointIndex = pointIndex;
        }
    }

    /// <summary>
    /// Fired by the hunted when beeing killed
    /// </summary>
    public class HuntedFinishedObjectivePhoMsg : PhotonMessage
    {
    }

    /// <summary>
    /// Called by any player when collecting an item
    /// </summary>
    public class HunterCollectedTrapPhoMsg : PhotonMessage
    {
        [JsonIgnore]
        public int playerNumber => n;

        public int n; // player id 

        public HunterCollectedTrapPhoMsg() { }
        public HunterCollectedTrapPhoMsg(int playerNumber)
        {
            n = playerNumber;
        }
    }

}