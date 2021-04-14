using System.Collections.Generic; 
using UnityEngine;

namespace JoVei.Base.TickSystem
{
    [CreateAssetMenu(fileName = "TickSystem Config", menuName = "JoVei/TickSystemConfig")]
    public class TickSystemConfig : ScriptableObject
    {
        [SerializeField] [Range(0, 100)] float timeScale = 1;
        [SerializeField] TickRegionDrawer defaultRegion;
        [SerializeField] List<TickRegionDrawer> regions;

        public float GetTimeScale() { return timeScale; }
        public TickRegionDrawer GetDefaultRegionDrawer() { return defaultRegion; }
        public TickRegionDrawer[] GetRegionDrawer() { return regions.ToArray(); }
        
        [System.Serializable]
        public class TickRegionDrawer 
        {
            public string id;
            public TickUpdateType type;
            [Range(0, 100)] public float border = 0;
            [Range(0, 100)] public float scale = 1;
        }
    }

}
