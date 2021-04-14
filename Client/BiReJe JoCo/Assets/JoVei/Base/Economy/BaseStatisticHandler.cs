using System;
using System.Linq;
using System.Collections.Generic;
using JoVei.Base.Helper;

namespace JoVei.Base.Economy
{
    /// <summary>
    /// Container class to handle statistics definded by an enum 
    /// </summary>
    public abstract class BaseStatisticHandler <TStatistic, TValue> : BaseSystemAccessor
        where TStatistic : Enum
    {
        /// <summary>
        /// Called whenever a currncy amount as been changed 
        /// statistic + from value + to value 
        /// </summary>
        public event Action<TStatistic, TValue, TValue> onValueChanged;

        /// <summary>
        /// All handled statistics 
        /// </summary>
        public virtual Dictionary<TStatistic, TValue> RegisteredStatistics { get; protected set; }
            = new Dictionary<TStatistic, TValue>();

        #region Add / Remove
        public BaseStatisticHandler() { }

        /// <summary>
        /// Auto registers all possible statistics with initial value 
        /// </summary>
        public BaseStatisticHandler(TValue initialValue)
        {
            RegisterAll(initialValue, false);
        }

        /// <summary>
        /// Registers a new statistic to be handled
        /// </summary>
        public void RegisterStatistic(TStatistic statistic, TValue initialValue, bool callEvent = true)
        {
            // already registerd?
            if (RegisteredStatistics.ContainsKey(statistic))
            {
                DebugHelper.PrintFormatted("There is already a statistic registered for type {0}", statistic.ToString());
                return;
            }

            // add to dictionary
            RegisteredStatistics.Add(statistic, initialValue);

            // call event?
            if (callEvent)
                onValueChanged?.Invoke(statistic, default, initialValue);
        }

        /// <summary>
        /// Registers all possible statistics
        public void RegisterAll(TValue initialValue, bool callEvent = true)
        {
            foreach (TStatistic curCurrency in Enum.GetValues(typeof(TStatistic)))
                RegisterStatistic(curCurrency, initialValue, callEvent);
        }

        /// <summary>
        /// Unregister an statistic
        /// </summary>
        public void UnregisterStatistic(TStatistic statistic, bool callEvent = true) 
        {
            // already registerd?
            if (!RegisteredStatistics.ContainsKey(statistic))
            {
                DebugHelper.PrintFormatted("There is no a statistic registered for type {0}", statistic.ToString());
                return;
            }

            // call event?
            if (callEvent)
                onValueChanged?.Invoke(statistic, RegisteredStatistics[statistic], default);

            RegisteredStatistics.Remove(statistic);
        }

        /// <summary>
        /// Unregister all registered statistics
        /// </summary>
        public void UnregisterAll(bool callEvent = true)
        {
            foreach (var curCurrency in RegisteredStatistics.Keys.ToArray())
                UnregisterStatistic(curCurrency, callEvent);
        }
        #endregion

        #region Management
        /// <summary>
        /// All handled statistics 
        /// </summary>
        public virtual TStatistic[] AllStatistics { get { return RegisteredStatistics.Keys.ToArray(); } }

        /// <summary>
        /// Get or set statistic
        /// </summary>
        public TValue this[TStatistic statistic]
        {
            get
            {
                return Get(statistic);
            }
            set 
            {
                Set(statistic, value);
            }
        }

        /// <summary>
        /// Override the value of a statistic
        /// </summary>
        public TValue Get(TStatistic statistic)
        {
            if (!RegisteredStatistics.ContainsKey(statistic))
                throw new ArgumentException(string.Format("There is no statistic registered for type {0}", statistic));

            return RegisteredStatistics[statistic];
        }

        /// <summary>
        /// Override the value of a statistic
        /// </summary>
        public void Set(TStatistic statistic, TValue value, bool callEvent = true)
        {
            if (!RegisteredStatistics.ContainsKey(statistic))
                throw new ArgumentException(string.Format("There is no statistic registered for type {0}", statistic));

            var tmp = RegisteredStatistics[statistic];
            RegisteredStatistics[statistic] = value;

            if (callEvent)
                onValueChanged?.Invoke(statistic, tmp, value);
        }

        /// <summary>
        /// Add value to statistic
        /// </summary>
        public void Add(TStatistic statistic, TValue value, bool callEvent = true)
        {
            if (!RegisteredStatistics.ContainsKey(statistic))
                throw new ArgumentException(string.Format("There is no statistic registered for type {0}", statistic));

            var tmp = RegisteredStatistics[statistic];
            RegisteredStatistics[statistic] = HandleAddition(RegisteredStatistics[statistic], value);

            if (callEvent)
                onValueChanged?.Invoke(statistic, tmp, RegisteredStatistics[statistic]);
        }

        /// <summary>
        /// Subtract value from statistic
        /// </summary>
        public void Subtract(TStatistic statistic, TValue value, bool callEvent = true)
        {
            if (!RegisteredStatistics.ContainsKey(statistic))
                throw new ArgumentException(string.Format("There is no statistic registered for type {0}", statistic));

            var tmp = RegisteredStatistics[statistic];
            RegisteredStatistics[statistic] = HandleSubtraction(RegisteredStatistics[statistic], value);

            if (callEvent)
                onValueChanged?.Invoke(statistic, tmp, RegisteredStatistics[statistic]);
        }


        /// <summary>
        /// Call onValueChanged event for every registered statistic
        /// </summary>
        public void FireUpdate() 
        {
            foreach (var curStatistic in RegisteredStatistics)
            {
                FireUpdate(curStatistic.Key);
            }
        }

        /// <summary>
        /// Call onValueChanged for statistic
        /// </summary>
        public void FireUpdate(TStatistic statistic)
        {
            if (!RegisteredStatistics.ContainsKey(statistic))
                throw new ArgumentException(string.Format("There is no statistic registered for type {0}", statistic));

            onValueChanged?.Invoke(statistic, RegisteredStatistics[statistic], RegisteredStatistics[statistic]);
        }
        #endregion

        #region Abstract Member
        protected abstract TValue HandleAddition(TValue baseValue, TValue addValue);
        protected abstract TValue HandleSubtraction(TValue baseValue, TValue addValue);
        #endregion
    }
}