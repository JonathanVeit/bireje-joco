using BiReJeJoCo.Items;
using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    [System.Serializable]
    public class MatchConfig 
    {
        public string matchMode;  // mode of the match
        public Dictionary<int, PlayerRole> roles; // <actor number, role> 
        public Dictionary<int, int> spawnPos; // <actor number, spawn position index> 
        public int duration; // duration of the match in seconds 

        public int collectableSeed; // seed to generate instance Ids
        public List<CollectableSpawnConfig> collectables; // collectables to be spawned

        [Newtonsoft.Json.JsonIgnore]
        public MatchMode Mode => MatchModeMapping.GetMapping().GetElementForKey(matchMode);
        [Newtonsoft.Json.JsonIgnore]
        public string matchScene => MatchModeMapping.GetMapping().GetElementForKey(matchMode).gameScene;
        [Newtonsoft.Json.JsonIgnore]
        public MapConfig mapConfig => MapConfigMapping.GetMapping().GetElementForKey(matchScene);

    }
}