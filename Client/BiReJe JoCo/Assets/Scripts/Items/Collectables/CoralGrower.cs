using BiReJeJoCo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BiReJeJoCo
{
    public class CoralGrower : SystemBehaviour
    {
        [Header("Grower Settings")]
        [SerializeField] Transform spawnOrigin;
        [SerializeField] float range;
        [SerializeField] [Range(0.1f, 1)] float minScale;
        [SerializeField] [Range(0.1f,1)] float maxScale;
        [SerializeField] [Range(0, 90)] float randomRotation;
        [SerializeField] float growSpeed;
        [SerializeField] LayerMask targetLayer;
        [SerializeField] DestroyableCoral[] coralPrefabs;

        #region Grow
        public void Grow(int seed)
        {
            var rnd = new System.Random(seed);

            var collection = new CoralGrowConfig()
            {
                Crystals = new Dictionary<Transform, Vector3>(),
            };

            for (int i = 0; i < matchHandler.MatchConfig.Mode.coralsPerSpawn; i++)
            {
                CoralSpawnPoint? spawnPoint = null;

                while (!spawnPoint.HasValue)
                {
                    spawnPoint = CalculateRandomSpawnPoint(rnd);
                }

                var coral = SpawnCoral(spawnPoint.Value, rnd);
                collection.Crystals.Add(coral.transform, RandomScale(rnd));
                coral.transform.SetParent(collectablesManager.Root);

                coral.InitializeCollectable(collectablesManager.GetInstanceId(coral.UniqueId), -1);
                collectablesManager.RegisterCollectableItem(coral);
            }

            StartCoroutine(GrowCorals(collection));
        }

        private DestroyableCoral SpawnCoral(CoralSpawnPoint spawnPoint, System.Random rnd)
        {
            var prefab = coralPrefabs[rnd.Next(0, coralPrefabs.Length)];
            var coral = Instantiate(prefab, spawnPoint.point, Quaternion.identity);

            coral.transform.parent = spawnOrigin.parent;
            coral.transform.up = RandomizeDirection(spawnPoint.direction, rnd);
            coral.transform.localScale = Vector3.zero;
            return coral;
        }

        private CoralSpawnPoint? CalculateRandomSpawnPoint(System.Random rnd)
        {
            var origin = spawnOrigin.position;
            var dir = RandomDirection(rnd);

            var ray = new Ray(origin, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range, targetLayer))
            {
                return new CoralSpawnPoint()
                {
                    point = hit.point,
                    direction = hit.normal,
                    parent = hit.collider.transform,
                };
            }

            return null;
        }

        private Vector3 RandomDirection(System.Random rnd)
        {
            var x = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);
            var y = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);
            var z = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);

            return new Vector3((float)x, (float)y, (float)z);
        }

        private Vector3 RandomScale(System.Random rnd)
        {
            var scale = Mathf.Clamp((float)System.Math.Round(rnd.NextDouble(), 1), minScale, maxScale);

            return new Vector3(scale, scale, scale);
        }

        private Vector3 RandomizeDirection(Vector3 direction, System.Random rnd)
        {
            var x = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);
            var y = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);
            var z = System.Math.Round(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? 1 : -1), 1);

            direction.x += randomRotation * (float)x;
            direction.y += randomRotation * (float)y;
            direction.z += randomRotation * (float)z;

            return direction;
        }

        private IEnumerator GrowCorals(CoralGrowConfig collection)
        {
            while (true)
            {
                bool finished = false;

                foreach (var entry in collection.Crystals.ToArray())
                {
                    if (entry.Key == null)
                    {
                        collection.Crystals.Remove(entry.Key);
                        continue;
                    }

                    if (entry.Key.localScale == entry.Value)
                        continue;

                    entry.Key.localScale = Vector3.MoveTowards(entry.Key.localScale, entry.Value, growSpeed * Time.deltaTime);
                    finished = false;
                }

                if (finished) break;
                yield return null;
            }

            Destroy(this.gameObject);
        }

        private struct CoralSpawnPoint
        {
            public Vector3 point;
            public Vector3 direction;
            public Transform parent;
        }

        private struct CoralGrowConfig
        {
            public Dictionary<Transform, Vector3> Crystals { get; set; }
        }
        #endregion
    }
}