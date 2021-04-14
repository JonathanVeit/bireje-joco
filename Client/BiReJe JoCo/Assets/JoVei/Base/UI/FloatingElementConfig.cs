using UnityEngine;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Config to request floating elements 
    /// </summary>
    public class FloatingElementConfig : IFloatingElementConfig
    {
        public string PrefabId { get; private set; }
        public Transform Parent { get; private set; }
        public Transform Target { get; set; }

        public Vector2 Offset { get; set; }

        public FloatingElementConfig(string prefabId, Transform parent, Transform target) : this(prefabId, parent, target, Vector2.zero) { }

        public FloatingElementConfig(string prefabId, Transform parent, Transform target, Vector2 offset)
        {
            this.PrefabId = prefabId;
            this.Parent = parent;
            this.Target = target;
            this.Offset = offset;
        }
    }
}