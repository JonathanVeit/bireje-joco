using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Simple ui class to display some stats
    /// </summary>
    public class DebugStatsUI : BaseSystemSingleton<DebugStatsUI>
    {
        [Header("UI Elements")]
        [SerializeField] Text fpsTF;
        [SerializeField] Text averageFpsTF;

        [Header("Settings")]
        [SerializeField] [Range(0, 1)] float FPSUpdateRate = 0.5f;
        [SerializeField] [Range(1, 60)] float averageFPSUpdateRate = 30f;

        private static bool isVisible;
        private static List<float> fpsCache
            = new List<float>();

        #region Show/Hide
        protected override void Awake()
        {
            base.Awake();

            if (isVisible) 
                Show();
            else
                Hide();
        }

        public void Show() 
        {
            Instance.gameObject.SetActive(true);
            isVisible = true;

            StartCoroutine(CO_UpdateFPS());
            StartCoroutine(CO_UpdateAverageFPS());
        }

        public void Hide()
        {
            Instance.gameObject.SetActive(false);
            isVisible = false;

            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            _instance = null;
            StopAllCoroutines();
        }
        #endregion

        #region UI
        private IEnumerator CO_UpdateFPS ()
        {
            var waiter = new WaitForSeconds(FPSUpdateRate);

            while (true)
            {
                UpdateFPS();
                yield return waiter;
            }
        }

        private IEnumerator CO_UpdateAverageFPS()
        {
            var waiter = new WaitForSeconds(averageFPSUpdateRate);

            while (true)
            {
                UpdateAverageFPS();
                yield return waiter;
            }
        }

        private void UpdateFPS()
        {
            var fps = (1 / Time.deltaTime);
            fpsCache.Add(fps);
            fpsTF.text = fps.ToString("F1") + " fps";
        }

        private void UpdateAverageFPS()
        {
            float sum = 0;
            foreach (var curValue in fpsCache) sum += curValue;
            sum /= fpsCache.Count;

            averageFpsTF.text = "~" + sum.ToString("F1") + " fps";
            fpsCache.Clear();
        }
        #endregion
    }
}