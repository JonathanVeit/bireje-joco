﻿using System;
using System.Collections;
using UnityEngine;

namespace JoVei.Base.Helper
{
    public enum TimerState
    {
        Finished = 1,
        Counting = 2,   
    }

    [System.Serializable]
    public class Timer
    {
        [SerializeField] float duration = 1;
        [SerializeField] float timeScale = 1;

        public float Duration { get { return duration; } set { duration = value; } }
        public float TimeScale { get { return timeScale; } set { timeScale = value; } }
        public TimerState State { get; private set; } = TimerState.Finished;
        public float Progress { get; private set; }
        public float RelativeProgress { get; private set; }

        public Timer() { }
        public Timer(float duration) : this(duration, 1) { }
        public Timer (float duration, float timeScale)
        {
            Duration = duration;
            TimeScale = timeScale;
        }

        private Coroutine counter;
        private Action onFinishedCallback;

        public void SetDuration(float duration)
        {
            Duration = duration;
        }
        public void SetTimeScale(float timeScale)
        {
            TimeScale = timeScale;
        }

        public void Start()
        {
            Start(null);
        }
        public void Start(Action onFinished) 
        {
            Start(null, onFinished);
        }
        public void Start(Action onUpdate, Action onFinished)
        {
            counter = CoroutineHelper.Instance.StartCoroutine(Count(onUpdate, onFinished));
        }
        public void Stop(bool fireCallback = false) 
        {
            if (counter != null)
            {
                CoroutineHelper.Instance.StopCoroutine(counter);
                if (fireCallback)
                    onFinishedCallback?.Invoke();
            }
        }

        private IEnumerator Count(Action onUpdate, Action onFinished)
        {
            State = TimerState.Counting;

            var counter = 0f;
            onFinishedCallback = () =>
            {
                Progress = Duration;
                RelativeProgress = 1;

                State = TimerState.Finished;
                onFinished?.Invoke();
                onFinishedCallback = null;
            };

            while (counter < Duration)
            {
                yield return null;
                counter += Time.deltaTime * timeScale;
                Progress = counter;
                RelativeProgress = counter / Duration;

                onUpdate?.Invoke();
            }
            onFinishedCallback?.Invoke();
        }
    }
}
