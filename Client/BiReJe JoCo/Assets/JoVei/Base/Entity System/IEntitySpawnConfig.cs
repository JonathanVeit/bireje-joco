using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Container for all relevant information to spawn an entity 
    /// </summary>
    public interface IEntitySpawnConfig 
    {
        // Id of the prefab
        public string PrefabId { get; }

        // config of the entity 
        public IEntityConfig EntityConfig { get; }

        #region Additional (not required)
        // parent to set 
        public Transform Parent { get; }

        // position to spawn at 
        public Vector3? WorldPosition { get; }

        // Transform.Forward
        public Vector3? ForwardDirection { get; }

        // Transform.Right
        public Vector3? RightDirection { get; }

        // layer to set 
        public int? Layer { get; }
        #endregion
    }
}