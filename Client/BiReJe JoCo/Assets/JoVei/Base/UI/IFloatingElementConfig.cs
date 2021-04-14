using UnityEngine;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Data required to create and manage floating elements 
    /// </summary>
    public interface IFloatingElementConfig
    {
        public string PrefabId { get; } // Id
        public Transform Parent { get; } // parent of floaty
        public Transform Target { get; } // target to follow
        public Vector2 Offset { get; } // offset to target 
    }
}