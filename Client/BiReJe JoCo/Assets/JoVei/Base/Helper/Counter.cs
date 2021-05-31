using System;
using UnityEngine;

namespace JoVei.Base.Helper
{
    [Serializable]
    public class Counter
    {
        [SerializeField] float maxValue = 1;
        [SerializeField] float timeScale = 1;

        public float MaxValue { get { return maxValue; } private set { maxValue = value; } }
        public float TimeScale { get { return timeScale; } private set { timeScale = value; } }
        public float Progress => counter;
        public float RelativeProgress => counter / maxValue;


        public Counter() { }
        public Counter(float maxValue) : this(maxValue, 1) { }
        public Counter(float maxValue, float timeScale)
        {
            MaxValue = maxValue;
            TimeScale = timeScale;
        }

        private float counter = 0;

        public void SetMaxValue(float maxValue)
        {
            MaxValue = maxValue;
        }
        public void SetTimeScale(float timeScale)
        {
            TimeScale = timeScale;
        }
        public void SetValue(float value)
        {
            counter = value;
        }

        public void CountUp(Action onReachedMaxCallback, bool autoReset = true)
        {
            counter = Mathf.Clamp(counter + Time.deltaTime * timeScale, 0, MaxValue);

            if (counter == maxValue)
            {
                onReachedMaxCallback?.Invoke();

                if (autoReset)
                    counter = 0;
            }
        }
        public void CountDown(Action onReachedZeroCallback, bool autoReset = true)
        {
            counter = Mathf.Clamp(counter - Time.deltaTime * timeScale, 0, MaxValue);

            if (counter == 0)
            {
                onReachedZeroCallback?.Invoke();
                
                if (autoReset)
                    counter = maxValue;
            }
        }
    }
}
