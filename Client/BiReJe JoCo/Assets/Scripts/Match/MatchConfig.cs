using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    [System.Serializable]
    public class MatchConfig 
    {
        public string matchScene; // scene to load 
        public Dictionary<int, PlayerRole> roles; // <actor number, role> 
        public Dictionary<int, int> spawnPos; // <actor number, spawn position index> 
    }
}