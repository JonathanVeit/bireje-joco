using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// EntitySpawnConfig for projectiles 
    /// </summary>
    public class ProjectileSpawnConfig : IEntitySpawnConfig
    {
        public string PrefabId { get; set; }
        public IEntityConfig EntityConfig { get; set; }
        public Transform Parent { get; set; }
        public Vector3? WorldPosition { get; set; }
        public Vector3? ForwardDirection { get; set; }
        public Vector3? RightDirection { get; set; }
        public int? Layer { get; set; }
    }
}