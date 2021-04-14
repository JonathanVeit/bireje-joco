using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.Helper
{
    /// <summary>
    /// Several usefull math functions
    /// </summary>
    public class MathHelper : MonoBehaviour
    {
        /// <summary>
        /// Clamps a euler angle to 0-360 degrees
        /// </summary>
        public static float ClampEuler360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }
            return result;
        }

        /// <summary>
        /// Returns a random point within a circle
        /// </summary>
        public static Vector3 RandomPointInCircle(float radius) 
        {
            return RandomPointInCircle(0, radius);
        }

        /// <summary>
        /// Returns a random point within a circle
        /// </summary>
        public static Vector3 RandomPointInCircle(float minRadius, float maxRadius)
        {
            var rawPoint = Random.insideUnitCircle;
            var multiplier = Random.Range(minRadius, maxRadius);
            rawPoint *= multiplier;
            return rawPoint;
        }

        /// <summary>
        /// Returns index of the element in the array
        /// </summary>
        public static int GetIndexInArray<TValue>(TValue[] array, TValue element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element))
                    return i;
            }

            return -1;
        }
    }
}