using System;
using System.Linq;
using System.Collections.Generic;
using JoVei.Base.Helper;

namespace JoVei.Base.Economy
{
    /// <summary>
    /// Container class to handle currencies definded by an enum 
    /// </summary>
    public abstract class BaseCurrencyHandler <TCurrency, TValue> : BaseSystemAccessor
        where TCurrency : Enum
    {
        /// <summary>
        /// Called whenever a currency amount as been changed 
        /// Currency + from value + to value 
        /// </summary>
        public event Action<TCurrency, TValue, TValue> onValueChanged;

        /// <summary>
        /// All handled currencies 
        /// </summary>
        public virtual Dictionary<TCurrency, TValue> RegisteredCurrencies { get; protected set; }
            = new Dictionary<TCurrency, TValue>();

        #region Add / Remove
        public BaseCurrencyHandler() { }

        /// <summary>
        /// Auto registers all possible currencies with initial value 
        /// </summary>
        public BaseCurrencyHandler(TValue initialValue)
        {
            RegisterAll(initialValue, false);
        }

        /// <summary>
        /// Registers a new currency to be handled
        /// </summary>
        public void RegisterCurrency(TCurrency currency, TValue deposite, bool callEvent = true)
        {
            // already registerd?
            if (RegisteredCurrencies.ContainsKey(currency))
            {
                DebugHelper.PrintFormatted("There is already a currency registered for type {0}", currency.ToString());
                return;
            }

            // add to dictionary
            RegisteredCurrencies.Add(currency, deposite);

            // call event?
            if (callEvent)
                onValueChanged?.Invoke(currency, default, deposite);
        }

        /// <summary>
        /// Registers all possible currencies
        public void RegisterAll(TValue initialValue, bool callEvent = true)
        {
            foreach (TCurrency curCurrency in Enum.GetValues(typeof(TCurrency)))
                RegisterCurrency(curCurrency, initialValue, callEvent);
        }

        /// <summary>
        /// Unregister an currency
        /// </summary>
        public void UnregisterCurrency(TCurrency currency, bool callEvent = true) 
        {
            // already registerd?
            if (!RegisteredCurrencies.ContainsKey(currency))
            {
                DebugHelper.PrintFormatted("There is no a currency registered for type {0}", currency.ToString());
                return;
            }

            // call event?
            if (callEvent)
                onValueChanged?.Invoke(currency, RegisteredCurrencies[currency], default);

            RegisteredCurrencies.Remove(currency);
        }

        /// <summary>
        /// Unregister all registered currencies
        /// </summary>
        public void UnregisterAll(bool callEvent = true)
        {
            foreach (var curCurrency in RegisteredCurrencies.Keys.ToArray())
                UnregisterCurrency(curCurrency, callEvent);
        }
        #endregion

        #region Management
        /// <summary>
        /// All handled Currencies 
        /// </summary>
        public virtual TCurrency[] AllCurrencies { get { return RegisteredCurrencies.Keys.ToArray(); } }

        /// <summary>
        /// Get or set currency
        /// </summary>
        public TValue this[TCurrency currency]
        {
            get
            {
                return Get(currency);
            }
            set 
            {
                Set(currency, value);
            }
        }

        /// <summary>
        /// Override the value of a currency
        /// </summary>
        public TValue Get(TCurrency currency)
        {
            if (!RegisteredCurrencies.ContainsKey(currency))
                throw new ArgumentException(string.Format("There is no currency registered for type {0}", currency));

            return RegisteredCurrencies[currency];
        }

        /// <summary>
        /// Override the value of a currency
        /// </summary>
        public void Set(TCurrency currency, TValue value, bool callEvent = true)
        {
            if (!RegisteredCurrencies.ContainsKey(currency))
                throw new ArgumentException(string.Format("There is no currency registered for type {0}", currency));

            var tmp = RegisteredCurrencies[currency];
            RegisteredCurrencies[currency] = value;

            if (callEvent)
                onValueChanged?.Invoke(currency, tmp, value);
        }

        /// <summary>
        /// Add value to currency
        /// </summary>
        public void Add(TCurrency currency, TValue value, bool callEvent = true)
        {
            if (!RegisteredCurrencies.ContainsKey(currency))
                throw new ArgumentException(string.Format("There is no currency registered for type {0}", currency));

            var tmp = RegisteredCurrencies[currency];
            RegisteredCurrencies[currency] = HandleAddition(RegisteredCurrencies[currency], value);

            if (callEvent)
                onValueChanged?.Invoke(currency, tmp, RegisteredCurrencies[currency]);
        }

        /// <summary>
        /// Subtract value from currency
        /// </summary>
        public void Subtract(TCurrency currency, TValue value, bool callEvent = true)
        {
            if (!RegisteredCurrencies.ContainsKey(currency))
                throw new ArgumentException(string.Format("There is no currency registered for type {0}", currency));

            var tmp = RegisteredCurrencies[currency];
            RegisteredCurrencies[currency] = HandleSubtraction(RegisteredCurrencies[currency], value);

            if (callEvent)
                onValueChanged?.Invoke(currency, tmp, RegisteredCurrencies[currency]);
        }

        /// <summary>
        /// Call onValueChanged event for every registered currency
        /// </summary>
        public void FireUpdate() 
        {
            foreach (var curCurrency in RegisteredCurrencies)
            {
                FireUpdate(curCurrency.Key);
            }
        }

        /// <summary>
        /// Call onValueChanged for currency
        /// </summary>
        public void FireUpdate(TCurrency currency)
        {
            if (!RegisteredCurrencies.ContainsKey(currency))
                throw new ArgumentException(string.Format("There is no currency registered for type {0}", currency));

            onValueChanged?.Invoke(currency, RegisteredCurrencies[currency], RegisteredCurrencies[currency]);
        }
        #endregion

        #region Abstract Member
        protected abstract TValue HandleAddition(TValue baseValue, TValue addValue);
        protected abstract TValue HandleSubtraction(TValue baseValue, TValue subtractValue);
        #endregion
    }
}