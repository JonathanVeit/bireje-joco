using UnityEngine;

namespace JoVei.Base.Helper
{
    /// <summary>
    /// Helps sorting 2D elements
    /// </summary>
    public static class ScreenSorting
    {
        public static int GetSortingForTransform(Transform transform) 
        {
            if (UnityEngine.Camera.main == null)
                return 0;

            // get position on screen
            var screenPos = UnityEngine.Camera.main.WorldToScreenPoint(transform.position);

            return (Screen.height - (int) screenPos.y);
        }
    }
}