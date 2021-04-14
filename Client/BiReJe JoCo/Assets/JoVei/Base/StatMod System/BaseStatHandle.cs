using System;
using System.Collections.Generic;
using UnityEngine;
using JoVei.Base.Helper;

namespace JoVei.Base.StatmodSystem
{
    /// <summary>
    /// Handles stats for a type with given origin, category and operator
    /// </summary>
    public abstract class BaseStatHandle<TStatType, TOrigin, TCategory, TMod>
        where TStatType : struct, Enum
        where TOrigin : struct, Enum
        where TCategory : struct ,Enum
        where TMod : IStatMod<TStatType, TOrigin, TCategory>
    {
        protected List<TMod> Mods { get; set; } 
            = new List<TMod>();

        protected Dictionary<TStatType, float> BaseStats { get; set; } 
            = new Dictionary<TStatType, float>();

        #region Add/Remove
        /// <summary>
        /// A base stat where mods will be applied to
        /// </summary>
        public virtual void RegisterBaseStat(TStatType type, float value)
        {
            if (!BaseStats.ContainsKey(type))
                BaseStats.Add(type, 0);

            BaseStats[type] = value;
        }

        /// <summary>
        /// Register each possible stat with value 0
        /// </summary>
        public virtual void RegisterDefaultBaseStats() 
        {
            RegisterDefaultBaseStats(0);
        }

        /// <summary>
        /// Register each possible stat with given value
        /// </summary>
        public virtual void RegisterDefaultBaseStats(float value)
        {
            foreach (TStatType curType in  Enum.GetValues(typeof(TStatType)))
            {
                RegisterBaseStat(curType, value);
            }
        }

        /// <summary>
        /// Consider removing the mods as well 
        /// </summary>
        public virtual void RemoveBaseStat(TStatType type)
        {
            BaseStats.Remove(type);
        }

        /// <summary>
        /// Modifications for given base stats
        /// </summary>
        public virtual void AddStatMod(TMod mod)
        {
            Mods.Add(mod);
        }

        /// <summary>
        /// Removes the given mod 
        /// </summary>
        public virtual void RemoveStatMod(TMod mod)
        {
            Mods.Remove(mod);
        }

        /// <summary>
        /// Removes the mod with the given Id
        /// </summary>
        public virtual void RemoveStatMod(string Id)
        {
            Mods.Remove(Mods.Find((x) => x.Id == Id));
        }
        #endregion

        #region Calculations
        /// <summary>
        /// Raw stat without mods
        /// </summary>
        public virtual float GetRawStat(TStatType type)
        {
            if (HasBaseStat(type)) return BaseStats[type];

            DebugHelper.PrintFormatted(LogType.Warning, "There is no BaseStat registered for {0}. Therefore 0 is beeing returned.", type.ToString());
            return 0;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual float GetStat(TStatType type)
        {
            float baseValue = GetRawStat(type);
            float additiveValue = 0;
            float increaseValue = 1;

            foreach (var curMod in GetMods(type, null, null))
            {
                switch (curMod.Operator)
                {
                    case StatModOperator.Add:
                        additiveValue += curMod.Value;
                        break;
                    case StatModOperator.Subtract:
                        additiveValue -= curMod.Value;
                        break;
                    case StatModOperator.Increase:
                        increaseValue += curMod.Value;
                        break;
                    case StatModOperator.Decrease:
                        increaseValue -= curMod.Value;
                        break;
                }
            }

            increaseValue = Mathf.Clamp(increaseValue, 0, float.MaxValue);

            baseValue += additiveValue;
            baseValue *= increaseValue;

            return baseValue;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual float GetStat(TStatType type, TOrigin origin)
        {
            float baseValue = GetRawStat(type);
            float additiveValue = 0;
            float increaseValue = 1;

            foreach (var curMod in GetMods(type, origin, null))
            {
                switch (curMod.Operator)
                {
                    case StatModOperator.Add:
                        additiveValue += curMod.Value;
                        break;
                    case StatModOperator.Subtract:
                        additiveValue -= curMod.Value;
                        break;
                    case StatModOperator.Increase:
                        increaseValue += curMod.Value;
                        break;
                    case StatModOperator.Decrease:
                        increaseValue -= curMod.Value;
                        break;
                }
            }

            increaseValue = Mathf.Clamp(increaseValue, 0, float.MaxValue);

            baseValue += additiveValue;
            baseValue *= increaseValue;

            return baseValue;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual float GetStat(TStatType type, TCategory category)
        {
            float baseValue = GetRawStat(type);
            float additiveValue = 0;
            float increaseValue = 1;

            foreach (var curMod in GetMods(type, null, category))
            {
                switch (curMod.Operator)
                {
                    case StatModOperator.Add:
                        additiveValue += curMod.Value;
                        break;
                    case StatModOperator.Subtract:
                        additiveValue -= curMod.Value;
                        break;
                    case StatModOperator.Increase:
                        increaseValue += curMod.Value;
                        break;
                    case StatModOperator.Decrease:
                        increaseValue -= curMod.Value;
                        break;
                }
            }

            increaseValue = Mathf.Clamp(increaseValue, 0, float.MaxValue);

            baseValue += additiveValue;
            baseValue *= increaseValue;

            return baseValue;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual float GetStat(TStatType type, TOrigin origin, TCategory category)
        {
            float baseValue = GetRawStat(type);
            float additiveValue = 0;
            float increaseValue = 1;

            foreach (var curMod in GetMods(type, origin, category))
            {
                switch (curMod.Operator)
                {
                    case StatModOperator.Add:
                        additiveValue += curMod.Value;
                        break;
                    case StatModOperator.Subtract:
                        additiveValue -= curMod.Value;
                        break;
                    case StatModOperator.Increase:
                        increaseValue += curMod.Value;
                        break;
                    case StatModOperator.Decrease:
                        increaseValue -= curMod.Value;
                        break;
                }
            }

            increaseValue = Mathf.Clamp(increaseValue, 0, float.MaxValue);

            baseValue += additiveValue;
            baseValue *= increaseValue;

            return baseValue;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual float ApplyModsTo(float value, TStatType type)
        {
            float baseValue = value;
            float additiveValue = 0;
            float increaseValue = 1;

            foreach (var curMod in GetMods(type, null, null))
            {
                switch (curMod.Operator)
                {
                    case StatModOperator.Add:
                        additiveValue += curMod.Value;
                        break;
                    case StatModOperator.Subtract:
                        additiveValue -= curMod.Value;
                        break;
                    case StatModOperator.Increase:
                        increaseValue += curMod.Value;
                        break;
                    case StatModOperator.Decrease:
                        increaseValue -= curMod.Value;
                        break;
                }
            }

            increaseValue = Mathf.Clamp(increaseValue, 0, float.MaxValue);

            baseValue += additiveValue;
            baseValue *= increaseValue;

            return baseValue;
        }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual bool HasBaseStat(TStatType type) { return BaseStats.ContainsKey(type); }

        /// <summary>
        /// Base stat with mods
        /// </summary>
        public virtual bool HasStatModFor(TStatType type) 
        {
            foreach (var curMod in Mods)
            {
                if (curMod.Type.Equals (type)) return true;
            }

            return false;
        }
        #endregion

        #region Helper
        public virtual TMod[] GetMods(TStatType type, TOrigin? origin, TCategory? category) 
        {
            var result = new List<TMod>();

            foreach (var curMod in Mods)
            {
                if (curMod.Type.Equals(type))
                {
                    if (origin != null)
                    {
                        if (!curMod.Origin.Equals(origin)) continue;    
                    }

                    if (category != null)
                    {
                        if (!curMod.Category.Equals(category)) continue;
                    }

                    result.Add(curMod);
                }
            }

            return result.ToArray();
        }
        #endregion
    }
}
