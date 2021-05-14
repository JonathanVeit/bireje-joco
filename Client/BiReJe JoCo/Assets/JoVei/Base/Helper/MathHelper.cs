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

        /// <summary>
        /// Returns true if there is an intersection between the two lines 
        /// </summary>
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 lineOrigin1,
                Vector3 lineDirection1, Vector3 lineOrigin2, Vector3 lineDirection2)
        {

            Vector3 lineVec3 = lineOrigin2 - lineOrigin1;
            Vector3 crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Approximately(planarFactor, 0f) &&
                !Mathf.Approximately(crossVec1and2.sqrMagnitude, 0f))
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = lineOrigin1 + (lineDirection1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }
    }
}