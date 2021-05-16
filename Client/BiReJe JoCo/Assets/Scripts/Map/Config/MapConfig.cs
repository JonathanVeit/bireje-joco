using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Config/MapConfig")]
    public class MapConfig : ScriptableObject
    {
        [SerializeField] Vector3[] hunterSpawnPoints;
        [SerializeField] Vector3[] huntedSpawnPoints;
        [SerializeField] Vector3[] collectableSpawnPoints;

        public int[] GetRandomHunterSpawnPointIndeces(int amount, bool noDuplicated = true)
        {
            var result = new List<int>();

            for (int i = 0; i < amount; i++)
            {
                while (true)
                {
                    var randomIndex = GetRandomHunterSpawnPointIndex();

                    if (noDuplicated && result.Contains(randomIndex) &&
                        amount <= hunterSpawnPoints.Length)
                        continue;

                    result.Add(randomIndex);
                    break;
                }
            }

            return result.ToArray();
        }
        public int GetRandomHunterSpawnPointIndex()
        {
            return Random.Range(0, hunterSpawnPoints.Length);
        }
        public int GetRandomHuntedSpawnPointIndex()
        {
            return Random.Range(0, huntedSpawnPoints.Length);
        }
        
        public int[] GetRandomCollectableSpawnPointIndices(int amount, bool noDuplicated = true)
        {
            var result = new List<int>();

            for (int i = 0; i < amount; i++)
            {
                while (true)
                {
                    var randomIndex = GetRandomCollectableSpawnPointIndex();

                    if (noDuplicated && result.Contains(randomIndex) &&
                        amount <= collectableSpawnPoints.Length)
                        continue;

                    result.Add(randomIndex);
                    break;
                }
            }

            return result.ToArray();
        }
        public int GetRandomCollectableSpawnPointIndex()
        {
            return Random.Range(0, collectableSpawnPoints.Length);
        }
        
        public Vector3 GetHunterSpawnPoint(int index)
        {
            return hunterSpawnPoints[index];
        }
        public Vector3 GetHuntedSpawnPoint(int index)
        {
            return huntedSpawnPoints[index];
        }
        
        public Vector3 GetCollectableSpawnPoint(int index)
        {
            return collectableSpawnPoints[index];
        }

        #region Override
        public void OverrideHunterSpawnPoints(Vector3[] points) 
        {
            hunterSpawnPoints = points;
        }
        public void OverrideHuntedSpawnPoints(Vector3[] points)
        {
            huntedSpawnPoints = points;
        }
        public void OverrideCollectableSpawnPoints(Vector3[] points)
        {
            collectableSpawnPoints = points;
        }
        #endregion
    }
}