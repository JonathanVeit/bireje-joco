using JoVei.Base.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace JoVei.Base.Data
{
    /// <summary>
    /// The base for a GameDataManager 
    /// </summary>
    public abstract class BaseGameDataManager : IGameDataManager, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            DataByType = new Dictionary<string, Dictionary<string, object>>();

            yield return LoadData();

            DIContainer.RegisterImplementation<IGameDataManager>(this);
        }

        protected abstract IEnumerator LoadData();

        public void CleanUp()
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<string, Dictionary<string, object>> DataByType;

        public IEnumerator LoadDataAsync<TData>(string location) where TData : BaseData 
        {
            string typeName = typeof(TData).Name;

            if (DataByType.ContainsKey(typeName))
            {
                DebugHelper.PrintFormatted(LogType.Error, "Data for type '{0}' has already been added.", typeName);
                yield break;
            }

            string serializedData = Resources.Load<TextAsset>(location + typeName).text;

            var settings = CreateSettings();
            var dataAsList = JsonConvert.DeserializeObject <List<TData>> (serializedData, settings);

            var dataAsDic = new Dictionary<string, object>();

            foreach (var curData in dataAsList)
            {
                if (string.IsNullOrEmpty(curData.UniqueId)) continue;

                dataAsDic.Add(curData.UniqueId, curData);
            }

            DataByType.Add(typeName, dataAsDic);
            DebugHelper.PrintFormatted(LogType.Log, "Loaded Game Data {0} with {1} entries", typeName, dataAsList.Count.ToString());
            yield return 0;
        }

        public bool GetDataForElement<TData>(string element, out TData result) where TData : BaseData
        {
            string typeName = typeof(TData).Name;

            if (!DataByType.ContainsKey(typeName))
            {
                DebugHelper.PrintFormatted(LogType.Error, "Cannot find entry for type {0}", typeName);
                result = default(TData);
                return false;
            }
            if (!DataByType[typeName].ContainsKey(element))
            {
                DebugHelper.PrintFormatted(LogType.Error, "Cannot find entry {0} for type {1}", element, typeName);
                result = default(TData);
                return false;
            }
            result = DataByType[typeName][element] as TData;
            return true;
        }

        public bool GetDataDictionary<TData>(out Dictionary<string, TData> dataAsDic) where TData : BaseData
        {
            string typeName = typeof(TData).Name;

            if (!DataByType.ContainsKey(typeName))
            {
                dataAsDic = default(Dictionary<string, TData>);
                return false;
            }

            dataAsDic = new Dictionary<string, TData>();
            foreach (var key in DataByType[typeName].Keys)
            {
                dataAsDic[key] = DataByType[typeName][key] as TData;
            }
            return true;
        }

        public bool GetDataArray<TData>(out TData[] result) where TData : BaseData
        {
            string typeName = typeof(TData).Name;

            if (!DataByType.ContainsKey(typeName))
            {
                result = default(TData[]);
                return false;
            }

            var rawDic = DataByType[typeName].Values.ToArray();
            var dataAsArray = new TData[rawDic.Length];

            for (int i = 0; i < rawDic.Length; i++)
            {
                dataAsArray[i] = rawDic[i] as TData;
            }

            result = dataAsArray;
            return true;
        }

        #region Abstract Methods
        protected abstract JsonSerializerSettings CreateSettings();
        #endregion
    }
}