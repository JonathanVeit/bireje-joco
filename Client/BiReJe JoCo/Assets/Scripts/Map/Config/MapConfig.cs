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
        [SerializeField] Floor[] floors;

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

        public int GetCollectableSpawnPointCount() 
        {
            return collectableSpawnPoints.Length;
        }
        public int[] GetRandomCollectableSpawnPointIndices(int amount, bool noDuplicated = true, float minDist = 1f)
        {
            var result = new List<int>();

            for (int i = 0; i < amount; i++)
            {
                while (true)
                {
                    var randomIndex = GetRandomCollectableSpawnPointIndex();

                    if (amount <= collectableSpawnPoints.Length &&
                        noDuplicated && 
                        result.Contains(randomIndex))
                        continue;
                    if (amount <= collectableSpawnPoints.Length &&
                        IsInRange(randomIndex, result, collectableSpawnPoints, minDist))
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

        public string GetFloorName(Vector3 position)
        {
            string floorName = string.Empty;
            foreach (var floor in floors)
            {
                if (position.y < floor.startsAtHeight)
                    floorName = floor.name;
            }

            return floorName;
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

        #region Helper
        private bool IsInRange(int index, IEnumerable<int> existingIndices, Vector3[] positions, float maxDistance)
        {
            foreach (var curIndex in existingIndices)
            {
                if (Vector3.Distance(positions[index], positions[curIndex]) <= maxDistance)
                    return true;
            }

            return false;
        }

        [System.Serializable]
        public struct Floor
        {
            public string name;
            public float startsAtHeight;
        }
        #endregion
    }
}