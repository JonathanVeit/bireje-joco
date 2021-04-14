using System.Collections;
using System.Collections.Generic;

namespace JoVei.Base.Data
{

    /// <summary>
    /// Handles loading, storing and accessing all data within the game
    /// </summary>
    public interface IGameDataManager
    {
        /// <summary>
        /// Get a specific element by its type and Id
        /// </summary>
        bool GetDataForElement<TData>(string element, out TData result) where TData : BaseData;
        
        /// <summary>
        /// Get all data as dictionary by its type
        /// </summary>
        bool GetDataDictionary<TData>(out Dictionary<string, TData> dataDic) where TData : BaseData;

        /// <summary>
        /// Get all data as array by its type
        /// </summary>
        bool GetDataArray<TData>(out TData[] result) where TData : BaseData;

        /// <summary>
        /// Load the serialzied data as given type 
        /// </summary>
        IEnumerator LoadDataAsync<TData>(string location) where TData : BaseData;
    }
}