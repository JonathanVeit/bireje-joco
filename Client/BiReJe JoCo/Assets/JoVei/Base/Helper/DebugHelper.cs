using UnityEngine;

namespace JoVei.Base.Helper
{
    public static class DebugHelper 
    {
        #region Logging
        public static void PrintFormatted(string message, params object[] args)
        {
            string msg = string.Format(message, args);

            Print(msg);
        }

        public static void PrintFormatted(LogType type, string message, params object[] args) 
        {
            string msg = string.Format(message, args);

            Print(type, msg);
        }

        public static void Print(string message)
        {
            Print(LogType.Log, message);
        }

        public static void Print(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case LogType.Exception:
                default:
                    PrintFormatted(LogType.Warning, "DebugHelper: Unhandled log type for message: {0} with type {1}!", message, type.ToString());
                    break;
            }
        }
        #endregion

        #region Rays
        public static void DrawCircle(Vector3 pos, Vector3 up, float radius, int segments, Color color, float duration = 0)
        {
            float angle = 0f;
            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                if (i > 0)
                {
                    Debug.DrawRay(lastPoint + pos, (thisPoint + pos) - (lastPoint + pos), color, duration);
                }

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }
        #endregion
    }
}