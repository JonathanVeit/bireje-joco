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
        [SerializeField] DestroyableCoral crystalPrefab;

        #region Grow
        public void Grow(int seed)
        {
            var rnd = new System.Random(seed);

            var collection = new CrystalGrowConfig()
            {
                Crystals = new Dictionary<Transform, Vector3>(),
            };

            for (int i = 0; i < matchHandler.MatchConfig.Mode.coralsPerSpawn; i++)
            {
                CrystalSpawnPoint? spawnPoint = null;

                while (!spawnPoint.HasValue)
                {
                    spawnPoint = CalculateRandomSpawnPoint(rnd);
                }

                var crystal = SpawnCristal(spawnPoint.Value, rnd);
                collection.Crystals.Add(crystal.transform, RandomScale(rnd));
                crystal.transform.SetParent(collectablesManager.Root);

                int id = rnd.Next();
                while (collectablesManager.HasCollectable(id.ToString()))
                    id = rnd.Next();
        
                crystal.InitializeCollectable(id.ToString(), -1);
                collectablesManager.RegisterCollectableItem(crystal);
            }

            StartCoroutine(GrowCrystals(collection));
        }

        private DestroyableCoral SpawnCristal(CrystalSpawnPoint spawnPoint, System.Random rnd)
        {
            var crystal = Instantiate(crystalPrefab, spawnPoint.point, Quaternion.identity);
            crystal.transform.up = RandomizeDirection(spawnPoint.direction, rnd);
            crystal.transform.localScale = Vector3.zero;
            return crystal;
        }

        private CrystalSpawnPoint? CalculateRandomSpawnPoint(System.Random rnd)
        {
            var origin = spawnOrigin.position;
            var dir = RandomDirection(rnd);

            var ray = new Ray(origin, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range, targetLayer))
            {
                return new CrystalSpawnPoint()
                {
                    point = hit.point,
                    direction = hit.normal,
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

        private IEnumerator GrowCrystals(CrystalGrowConfig collection)
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

        private struct CrystalSpawnPoint
        {
            public Vector3 point;
            public Vector3 direction;
        }

        private struct CrystalGrowConfig
        {
            public Dictionary<Transform, Vector3> Crystals { get; set; }
        }
        #endregion
    }
}