using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Implementation for IProjectileController with all required functionalities 
    /// Projectiles are handled as IEntity using the implemented ESC
    /// </summary>
    public class ProjectileController : BaseSystemAccessor, IProjectileController
    {
        /// <summary>
        /// All projectiles currently handled by the controller
        /// </summary>
        public List<IProjectile> Projectiles { get; private set; }
            = new List<IProjectile>();

        /// <summary>
        /// Createds a new projectile as entity 
        /// </summary>
        public virtual TProjectile FireProjectileAs<TProjectile>(string prefabId, IProjectileConfig config, Vector3 position, int layer)
            where TProjectile : IProjectile
        {
            return (TProjectile)FireProjectile(prefabId, config, position, layer);
        }

        /// <summary>
        /// Createds a new projectile as entity 
        /// </summary>
        public virtual IProjectile FireProjectile(string prefabId, IProjectileConfig config, Vector3 position, int layer)
        {
            // create spawn config
            var spawnConfig = new ProjectileSpawnConfig()
            {
                PrefabId = prefabId,
                EntityConfig = config,
                WorldPosition = position,
                Layer = layer,
            };

            // set controller
            config.Controller = this;

            // create projectile using the ECS
            var newProjectile = ECS.CreateEntity(spawnConfig) as IProjectile;
            Projectiles.Add(newProjectile);
            return newProjectile;
        }

        /// <summary>
        /// Destroys a certain projectile without fireing events 
        /// </summary>
        public virtual void DestroyProjectile(IProjectile projectile)
        {
            ECS.DestroyEntity(projectile as IEntity);
            Projectiles.Remove(projectile);
        }

        /// <summary>
        /// Destroys all projectiles currently handled
        /// </summary>
        public virtual void DestroyAll(Func<IProjectile, bool> condition)
        {
            foreach (var curProjectile in Projectiles.ToArray())
            {
                if (condition(curProjectile))
                    DestroyProjectile(curProjectile);
            }
        }

        /// <summary>
        /// Destroys all projectiles currently handled
        /// </summary>
        public virtual void DestroyAll()
        {
            foreach (var curProjectile in Projectiles.ToArray())
                DestroyProjectile(curProjectile);
        }

        /// <summary>
        /// Projectile will no longer be controlled by the Controller or the Entity System
        /// </summary>
        public void ReleaseProjectile(IProjectile projectile)
        {
            ECS.ReleaseEntity(projectile as IEntity);
            Projectiles.Remove(projectile);
        }

        /// <summary>
        /// Releases all projectiles controlled 
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var curProjectile in Projectiles.ToArray())
                ReleaseProjectile(curProjectile);
        }

        /// <summary>
        /// Releases all projectiles controlled 
        /// </summary>
        public void ReleaseAll(Func<IProjectile, bool> condition)
        {
            foreach (var curProjectile in Projectiles.ToArray())
            {
                if (condition(curProjectile))
                    ReleaseProjectile(curProjectile);
            }
        }

        /// <summary>
        /// Projectile will no longer be controlled by the Controller or the Entity System
        /// </summary>
        public void FreeProjectile(IProjectile projectile)
        {
            projectile.SetFree();
        }

        /// <summary>
        /// Releases all projectiles controlled 
        /// </summary>
        public void FreeAll()
        {
            foreach (var curProjectile in Projectiles.ToArray())
                FreeProjectile(curProjectile);
        }

        /// <summary>
        /// Releases all projectiles controlled 
        /// </summary>
        public void FreeAll(Func<IProjectile, bool> condition)
        {
            foreach (var curProjectile in Projectiles.ToArray())
            {
                if (condition(curProjectile))
                    FreeProjectile(curProjectile);
            }
        }
    }
}