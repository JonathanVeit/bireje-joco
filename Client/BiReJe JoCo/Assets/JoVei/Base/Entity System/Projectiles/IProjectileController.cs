using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.EntitySystem
{
    /// <summary>
    /// Controller to easily handle projectiles of any kind 
    /// </summary>
    public interface IProjectileController
    {
        List<IProjectile> Projectiles { get; }

        IProjectile FireProjectile(string prefabId, IProjectileConfig config, Vector3 position, int layer);
        TProjectile FireProjectileAs<TProjectile>(string prefabId, IProjectileConfig config, Vector3 position, int layer)
           where TProjectile : IProjectile;

        void DestroyProjectile(IProjectile projectile);
        void DestroyAll(Func<IProjectile, bool> condition);
        void DestroyAll();

        void ReleaseProjectile (IProjectile projectile);
        void ReleaseAll(Func<IProjectile, bool> condition);
        void ReleaseAll();

        void FreeProjectile(IProjectile projectile);
        void FreeAll(Func<IProjectile, bool> condition);
        void FreeAll();
    }
}