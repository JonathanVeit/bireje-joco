using System;
using UnityEngine;
using System.Collections;

namespace JoVei.Base.Helper
{
    public static class DateTimeHelper
    {
        private static DateTime _now;
        private static DateTime _utcNow;

        private static Coroutine timeUpdater;
        private static float frequence = 0;

        public static DateTime Now
        {
            get
            {
                CheckAndCreateTimeUpdater();

                return _now;
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                CheckAndCreateTimeUpdater();

                return _utcNow;
            }
        }

        private static IEnumerator UpdateTime() 
        {
            var waiter = new WaitForSeconds(frequence);

            while (true)
            {
                _now = DateTime.Now;
                _utcNow = DateTime.UtcNow;

                yield return waiter;
            }
        }

        #region Helper
        private static void CheckAndCreateTimeUpdater() 
        {
            if (timeUpdater == null)
                timeUpdater = CoroutineHelper.Instance.StartCoroutine(UpdateTime());
        }
        #endregion
    }
}