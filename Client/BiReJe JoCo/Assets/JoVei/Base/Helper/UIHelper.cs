using UnityEngine;

namespace JoVei.Base.Helper
{
    public static class UIHelper
    {
        public static Vector3 ClampScreenPointToScreenBorder(Vector3 point, bool invert = false)
        {
            point.z = 0;
            var halfWidth = Screen.width / 2;
            var halfHeight = Screen.height / 2;

            var screenCenter = new Vector3(halfWidth, halfHeight, 0);
            var directionToPoint = (point - screenCenter).normalized;
            
            if (directionToPoint.magnitude == 0)
                directionToPoint.x += 1;
            if (invert) 
                directionToPoint *= -1;

            var lowerLeftCorner = new Vector3(0, 0, 0);
            var upperLeftCorner = new Vector3(0, Screen.height, 0);
            var lowerRightCorner = new Vector3(Screen.width, 0, 0);

            var verticalBorderDirection = new Vector3(0, Screen.height, 0);
            var horizontalBorderDirection = new Vector3(Screen.width, 0, 0);
            Vector3 intersection;

            if (directionToPoint.y > 0)
            {
                // try upper horizontal border
                if (MathHelper.LineLineIntersection(out intersection,
                    upperLeftCorner, horizontalBorderDirection,
                    screenCenter, directionToPoint))
                {
                    // hit within screen border?
                    if (intersection.x <= Screen.width &&
                        intersection.x >= 0)
                    {
                        return intersection;
                    }
                }
            }
            else
            {
                // try lower horizontal border
                if (MathHelper.LineLineIntersection(out intersection,
                    lowerLeftCorner, horizontalBorderDirection,
                    screenCenter, directionToPoint))
                {
                    // hit within screen border?
                    if (intersection.x <= Screen.width &&
                        intersection.x >= 0)
                    {
                        return intersection;
                    }
                }
            }

            if (directionToPoint.x < 0)
            {
                // try left vertical border
                if (MathHelper.LineLineIntersection(out intersection,
                lowerLeftCorner, verticalBorderDirection,
                screenCenter, directionToPoint))
                {
                    // hit within screen border?
                    if (intersection.y <= Screen.height &&
                        intersection.y >= 0)
                    {
                        return intersection;
                    }
                }
            }
            else
            {
                // try right vertical border
                if (MathHelper.LineLineIntersection(out intersection,
                    lowerRightCorner, verticalBorderDirection,
                    screenCenter, directionToPoint))
                {
                    // hit within screen border?
                    if (intersection.y <= Screen.height &&
                        intersection.y >= 0)
                    {
                        return intersection;
                    }
                }
            }

            return point;
        }
    }
}
