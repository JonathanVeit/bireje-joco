using System;
using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Config for projetiles containing a layer, target and speed
    /// </summary>
    public interface IProjectileConfig : IEntityConfig
    {
        public IProjectileController Controller { get; set; } // owning controller
        public Transform Target { get; set; } // target to reach 
        public float Speed { get; } // move speed

        public Action<IProjectile,GameObject> OnProjectileHit { get; set; } // called when hit target
        public Action<IProjectile> OnProjectileOutOfRange { get; set; } // called when target is out of range 
    }
}