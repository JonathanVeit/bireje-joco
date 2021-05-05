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
            var characterModel = target.GetComponent<PlayerCharacterModel>();
            if (isLocalBullet && characterModel != null)
            {
                photonMsgHub.ShoutMessage<HuntedHitByBulletPhoMsg>(characterModel.Owner, damage);
            }
         
            SpawnEffect();

            GetComponent<Rigidbody>().isKinematic = true;
            RequestReturnToPool();
        }

        private void SpawnEffect() 
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("bullet_hit_sfx");
            poolingManager.PoolInstance(prefab, transform.position, Quaternion.identity);
        }
    }
}