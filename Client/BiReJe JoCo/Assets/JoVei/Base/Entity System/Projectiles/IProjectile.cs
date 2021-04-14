using System;
using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Projectile with target and speed as well es basic events 
    /// </summary>
    public interface IProjectile
    {
        // events 
        event Action<IProjectile, GameObject> OnProjectileHit;
        event Action<IProjectile> OnProjectileOutOfRange;

        // target to steer to
        Transform Target { get; }
        float Speed { get; }

        // free from target and owning controller
        void SetFree();
    }
}