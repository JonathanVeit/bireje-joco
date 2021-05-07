using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.Helper;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Controlls the fill amount of an image
    /// </summary>
    [System.Serializable]
    public class UIBarHandler
    {
        private enum InterpolationType
        {
            Lerp = 0,
            MoveToward = 1,
        }

        [SerializeField] InterpolationType type;
        [SerializeField] Image target;
        [SerializeField] float speed;

        public Image TargetImage { get { return target; } }

        #region Runtime Member
        private float currentValue;
        private float targetValue;
        private bool isFirst = true;
        #endregion

        private Coroutine handler;

        public void SetValue(float value)
        {
            targetValue = value;

            if (isFirst)
            {
                currentValue = target.fillAmount;
                isFirst = false;
            }

            CheckAndCreateHandler();
        }

        public void OverrideValue(float value)
        {
            currentValue = targetValue = value;
            CheckAndCreateHandler();
        }

        #region Helper
        private void CheckAndCreateHandler() 
        {
            if (target == null)
            {
                DebugHelper.Print(LogType.Error, "UI Bar Handler has no target");
                return;
            }

            if (handler == null)
                handler = CoroutineHelper.Instance.StartCoroutine(Handler());
        }

        private IEnumerator Handler()
        {
            while (true)
            {
                if (target == null) break;

                switch (type)
                {
                    case InterpolationType.Lerp:
                        currentValue = Mathf.Lerp(currentValue, targetValue, speed * Time.deltaTime);
                        break;
                    case InterpolationType.MoveToward:
                        currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);
                        break;
                }

                target.fillAmount = currentValue;

                // finish?
                if (Mathf.Abs(targetValue - currentValue) < 0.01f)
                {
                    handler = null;
                    break;
                }

                yield return null;
            }
        }
        #endregion
    }
}