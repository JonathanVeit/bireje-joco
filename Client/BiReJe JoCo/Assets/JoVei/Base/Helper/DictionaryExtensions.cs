using System;
using System.Collections.Generic;

namespace JoVei.Base.Helper
{
    public static class DictionaryExtensions
    {
        public static TKey KeyForValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (var curKey in dictionary.Keys)
            {
                if (dictionary[curKey].Equals(value) )
                    return curKey;
            }

            throw new ArgumentException($"Dictionary does not contain a key for {value}");
        }
    }
}
