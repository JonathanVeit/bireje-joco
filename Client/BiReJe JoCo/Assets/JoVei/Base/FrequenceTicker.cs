using System;
using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Frequence Ticker take a certain amount of time and whenever the Tick() method is called, they trigger a callback if the time was over
    /// The basic value is calculated in seconds 
    /// </summary>
    public class FrequenceTicker
    {
        public enum TickType { Milliseconds, Seconds, Minutes }

        public float Frequence { get; private set; }
        public bool RandomStart { get; private set; }
        public Action TickCallback { get; private set; }

        public float? Counter { get; private set; }

        public FrequenceTicker(float frequence, Action tickCallback) : this(frequence, TickType.Milliseconds, tickCallback) { }

        public FrequenceTicker(float frequence, TickType type, Action tickCallback) : this(frequence, type, false, tickCallback) { }

        public FrequenceTicker(float frequence, bool RandomStart, Action tickCallback) : this(frequence, TickType.Milliseconds, RandomStart, tickCallback) { }

        public FrequenceTicker(float frequence, TickType type, bool RandomStart, Action tickCallback)
        {
            switch (type)
            {
                case TickType.Milliseconds:
                    Frequence = frequence/1000;
                    break;
                case TickType.Seconds:
                    Frequence = frequence;
                    break;
                case TickType.Minutes:
                    Frequence = frequence * 60000;
                    break;
            }

            this.RandomStart = RandomStart;
            TickCallback += tickCallback;
        }

        public void Tick() 
        {
            Tick(Time.deltaTime);
        }

        public void Tick(float deltaTime)
        {
            if (Counter == null)
            {
                Counter = RandomStart ? UnityEngine.Random.Range(Frequence, 0) : 0;
            }
            else
            {
                Counter += deltaTime;
            }

            if ((Counter) >= Frequence)
            {
                GiveCallbackAndRestart();
            }

        }

        public void GiveCallback() 
        {
            TickCallback?.Invoke();
        }

        public void GiveCallbackAndRestart()
        {
            GiveCallback();
            Restart();
        }

        public void Restart() 
        {
            Counter = 0;
        }

        public void Reset()
        {
            Counter = null;
        }
    }
}