using System;
using System.Collections.Generic;
using System.Linq;

namespace JoVei.Base.Data
{
    /// <summary>
    /// Data with level progression
    /// </summary>
    public abstract class ProgressiveBaseData<TData> : BaseData 
        where TData : ProgressiveBaseData<TData>, new()
    {
        /// <summary>
        /// Cache of the data for each level
        /// </summary>
        protected static Dictionary<string, Dictionary<int, TData>> DataForLevel { get; private set; }

        /// <summary>
        /// Returns the data for the given level
        /// </summary>
        public TData this[int level]
        {
            get 
            {
                // initialize dic
                if (DataForLevel == null) DataForLevel = new Dictionary<string, Dictionary<int, TData>>();

                // add entry for this id 
                if (!DataForLevel.ContainsKey(UniqueId))
                {
                    DataForLevel.Add(UniqueId, new Dictionary<int, TData>());
                }

                // create entry for level
                if (!DataForLevel[UniqueId].ContainsKey(level))
                { 
                    // load the default object with lvl 1 stats
                    gameDataManager.GetDataForElement(UniqueId, out TData data);

                    // create a copy 
                    var newData = data.CreateCopy();

                    // initialize the data for the given level
                    InitializeForLevel(newData, level);

                    // add the copy to the cache
                    DataForLevel[UniqueId].Add(level, newData);
                }

                return DataForLevel[UniqueId][level];
            }
        }

        /// <summary>
        /// Initializes the given data with the stats of the given level 
        /// The given data has the values of the default data at lvl 1
        /// </summary>
        protected abstract void InitializeForLevel(TData data, int level);

        /// <summary>
        /// Creates a copy of the object
        /// </summary>
        /// <returns></returns>
        protected abstract TData CreateCopy();

        #region Helper
        protected float CalculateLevelProgression(float baseValue, int toLevel, Dictionary<int, float> progression, int roundDigits = 0)
        {
            // result
            float result = baseValue;

            // level per progression key -> how many level to be calculated with the multiplier
            var multiplierForLevel = new Dictionary<float, List<int>>();

            // progression keys as array
            var progressionLevel = progression.Keys.ToArray();

            // calculate level per progression key
            int calculatedLevel = 1;
            foreach (var curStep in progressionLevel)
            {
                // add entry for the multiplier
                if (!multiplierForLevel.ContainsKey(progression[curStep]))
                {
                    multiplierForLevel.Add(progression[curStep], new List<int>());
                }

                // step caps the level
                if (toLevel <= curStep)
                {
                    multiplierForLevel[progression[curStep]].Add(toLevel - calculatedLevel);
                    break;
                }
                // step does not cap the level
                else
                {
                    multiplierForLevel[progression[curStep]].Add(curStep - calculatedLevel);
                    calculatedLevel = curStep;
                }
            }

            // calculate final progression
            foreach (var curMultiplier in multiplierForLevel)
            {
                foreach (var curLevelStep in curMultiplier.Value)
                    result += CalculateValueProgression(baseValue, curMultiplier.Key, curLevelStep);
            }

            return (float) Math.Round (result, roundDigits);
        }

        protected float CalculateValueProgression(float baseValue, float multiplier, int steps)
        {
            // incrase for 1 step
            float addValue = baseValue * multiplier;

            // multiply with steps
            addValue *= steps;

            return addValue;
        }
        #endregion
    }
}