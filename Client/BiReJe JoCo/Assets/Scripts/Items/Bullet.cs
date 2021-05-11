using UnityEngine;
using JoVei.Base;
using JoVei.Base.PoolingSystem;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class Bullet : PoolablePrefab
    {
        [Header("Settings")]
        [SerializeField] float damage;
        [SerializeField] int ignoreCollisions;

        private bool isLocalBullet;
        private int collisions;
        private PhotonMessageHub photonMsgHub => DIContainer.GetImplementationFor<PhotonMessageHub>();
        private PlayerManager playerManager => DIContainer.GetImplementationFor<PlayerManager>();

        public void Initialize(bool isLocalBullet)
        {
            this.isLocalBullet = isLocalBullet;

            GetComponent<Rigidbody>().isKinematic = false;
            collisions = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            collisions++;

            if (collisions > ignoreCollisions)
            {
                HandleHit(collision.gameObject);
            }
        }

        private void HandleHit(GameObject target)
        {
            if (isLocalBullet)
            {
                HandleHitLocal(target);
            }

            SpawnEffect();

            GetComponent<Rigidbody>().isKinematic = true;
            RequestReturnToPool();
        }

        private void HandleHitLocal(GameObject target)
        {
            if (target.layer == 10)
            {
                photonMsgHub.ShoutMessage<HuntedHitByBulletPhoMsg>(playerManager.GetAllPlayer((x) => x.Role == PlayerRole.Hunted)[0], damage);
            }
        }

        private void SpawnEffect()
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("bullet_hit_sfx");
            poolingManager.PoolInstance(prefab, transform.position, Quaternion.identity);
        }
    }
}