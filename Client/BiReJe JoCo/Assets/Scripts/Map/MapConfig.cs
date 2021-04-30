using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Config/MapConfig")]
    public class MapConfig : ScriptableObject
    {
        [SerializeField] Vector3[] hunterSpawnPoints;
        [SerializeField] Vector3[] huntedSpawnPoints;

        public int[] GetRandomHunterSpawnPointIndex(int amount, bool noDuplicated = true)
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


        public Vector3 GetHunterSpawnPoint(int index)
        {
            return hunterSpawnPoints[index];
        }

        public Vector3 GetHuntedSpawnPoint(int index)
        {
            return huntedSpawnPoints[index];
        }
    }
}