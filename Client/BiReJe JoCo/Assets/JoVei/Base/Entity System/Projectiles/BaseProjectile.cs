using System;
using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Base implementation of IProjecile as an BaseEntity
    /// </summary>
    public abstract class BaseProjectile<TConfig> : BaseEntity<TConfig>, IProjectile
        where TConfig : IProjectileConfig
    {
        /// <summary>
        /// Called by the projectile when hitting any target 
        /// </summary>
        public event Action<IProjectile, GameObject> OnProjectileHit;

        /// <summary>
        /// Called by the projectile when its out of range 
        /// </summary>
        public event Action<IProjectile> OnProjectileOutOfRange;
        
        /// <summary>
        /// Target as defined in the config 
        /// </summary>
        public Transform Target => Config.Target;

        /// <summary>
        /// Speed as defined in the config 
        /// </summary>
        public float Speed => Config.Speed;

        public override void Initialize(string entityId, IEntitySpawnConfig spawnConfig, IEntityControlSystem owningSystem)
        {
            base.Initialize(entityId, spawnConfig, owningSystem);

            // subscribe to events 
            OnProjectileHit += Config.OnProjectileHit;
            OnProjectileOutOfRange += Config.OnProjectileOutOfRange;
        }

        /// <summary>
        /// To be called when hitting any target 
        /// Will release the entity immeditely
        /// </summary>
        protected virtual void Hit(GameObject target) 
        {
            OnProjectileHit?.Invoke(this, target);
            RequestDestroy();
        }

        /// <summary>
        /// To be called when the projecile is out of range and can be released 
        /// </summary>
        protected virtual void OutOfRange() 
        {
            OnProjectileOutOfRange?.Invoke(this);
            RequestDestroy();
        }

        /// <summary>
        /// The projectile will no longer aim at any target 
        /// </summary>
        public virtual void SetFree()
        {
        }

        /// <summary>
        /// Requests Destroy at the owning controller
        /// </summary>
        public override void RequestRelease()
        {
            Config.Controller.ReleaseProjectile(this);

        }

        /// <summary>
        /// Requests Destroy at the owning controller
        /// </summary>
        public override void RequestDestroy()
        {
            Config.Controller.DestroyProjectile(this);
        }
    }
}