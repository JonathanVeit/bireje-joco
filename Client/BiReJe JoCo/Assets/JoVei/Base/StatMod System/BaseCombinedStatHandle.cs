using System;
using System.Collections.Generic;

namespace JoVei.Base.StatmodSystem
{
    /// <summary>
    /// Combines a collection of StatHandles together with its own stats
    /// </summary>
    public abstract class BaseCombinedStatHandle<TStatType, TOrigin, TCategory, TMod, TSubHandle> : BaseStatHandle<TStatType, TOrigin, TCategory, TMod>
    where TStatType : struct, Enum
    where TOrigin : struct, Enum
    where TCategory : struct, Enum
    where TMod : IStatMod<TStatType, TOrigin, TCategory>
    where TSubHandle : BaseStatHandle<TStatType, TOrigin, TCategory, TMod>
    {
        protected List<TSubHandle> SubHandles { get; set; }
           = new List<TSubHandle>();

        #region Add/Remove
        public virtual void RegisterSubHandle(TSubHandle subHandle)
        {
            SubHandles.Add(subHandle);
        }

        public virtual void RemoveSubHandle(TSubHandle subHandle)
        {
            SubHandles.Remove(subHandle);
        }
        #endregion

        #region Calculations
        public override float GetRawStat(TStatType type)
        {
            if (HasBaseStat(type)) return BaseStats[type];

            // no warning log because it may be intended to not store base stats in the combined handler 
            return 0;
        }

        public virtual float GetRawCombinedStat(TStatType type)
        {
            float baseValue = GetRawStat(type);

            foreach (var curSubHandle in SubHandles)
            {
                if (curSubHandle.HasBaseStat(type)) baseValue += curSubHandle.GetRawStat(type);
            }

            return baseValue;
        }

        public virtual float GetCombinedStat(TStatType type)
        {
            float baseValue = base.GetStat(type);

            foreach (var curSubHandle in SubHandles)
                baseValue += curSubHandle.GetStat(type);

            return baseValue;
        }

        public virtual float GetCombinedStat(TStatType type, TOrigin origin)
        {
            float baseValue = base.GetStat(type, origin);

            foreach (var curSubHandle in SubHandles)
                baseValue += curSubHandle.GetStat(type, origin);

            return baseValue;
        }

        public virtual float GetCombinedStat(TStatType type, TCategory category)
        {
            float baseValue = base.GetStat(type, category);

            foreach (var curSubHandle in SubHandles)
                baseValue += curSubHandle.GetStat(type, category);

            return baseValue;
        }
        
        public virtual float GetCombinedStat(TStatType type, TOrigin origin, TCategory category)
        {
            float baseValue = base.GetStat(type, origin, category);

            foreach (var curSubHandle in SubHandles)
                baseValue += curSubHandle.GetStat(type, origin, category);

            return baseValue;
        }

        public virtual float ApplyCombinedModsTo(float value, TStatType type)
        {
            float baseValue = base.ApplyModsTo(value, type);

            foreach (var curSubHandle in SubHandles)
                baseValue += curSubHandle.ApplyModsTo(baseValue, type);

            return baseValue;
        }

        public virtual int RegisteredSubHandles() { return SubHandles.Count; }
        #endregion
    }
}