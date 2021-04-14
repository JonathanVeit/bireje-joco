using JoVei.Base.Helper;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JoVei.Base
{
    /// <summary>
    /// Base for mapping assets to a unique key
    /// </summary>
    public abstract class AssetMapper<TMapper, TKey, TAsset> : ScriptableObject 
        where TMapper : AssetMapper<TMapper, TKey, TAsset>
    {
        [SerializeField] List<MappingElement> MappedContent;
        private  Dictionary<TKey, TAsset> loadedAssets;
        private static TMapper loadedMapping;

        /// <summary>
        /// Get the element with the givene Id 
        /// <returns></returns>
        public virtual TAsset GetElementForKey(TKey Key)
        {
            // search element for id
            loadedAssets.TryGetValue(Key, out TAsset result);

            if (result != null) return result;

            DebugHelper.Print(LogType.Error, string.Format("Cannot find mapped asset for {0}", Key));
            return default;
        }

        /// <summary>
        /// Get all loaded elements
        /// <returns></returns>
        public virtual TAsset[] GetAllElements()
        {
            // search element for id
            return loadedAssets.Values.ToArray();
        }

        /// <summary>
        /// Get all loaded elements
        /// <returns></returns>
        public virtual Dictionary<TKey, TAsset> GetAllElementsAsDic()
        {
            // search element for id
            return loadedAssets;
        }

        #region Static Loading 
        /// <summary>
        /// Get mapping from resources 
        /// </summary>
        /// <returns></returns>
        public static TMapper GetMapping()
        {
            if (loadedMapping != null)
                return loadedMapping;

            TryLoadMapping();
            return loadedMapping;
        }

        private static void TryLoadMapping()
        {
            // load all mappings from resources 
            var typeName = typeof(TMapper).Name.ToString();
            var results = Resources.LoadAll<TMapper>("");

            if (results.Length == 0)
            {
                DebugHelper.Print(LogType.Error, string.Format("Cannot find mapping for {0}.", typeName));
                return;
            }

            // first result is stored as loaded mapping
            loadedMapping = results[0];

            // add all other mappings to the loaded one 
            for (int i = 0; i < results.Length; i++)
            {
                loadedMapping.AddMappedElements(results[i].MappedContent);
            }
        }
        #endregion

        #region Helper
        [System.Serializable]
        private struct MappingElement
        {
            public TKey ID;
            public TAsset Element;
        }

        private void AddMappedElements(List<MappingElement> elements)
        {
            if (loadedAssets == null)
                loadedAssets = new Dictionary<TKey, TAsset>();

            foreach (var curElement in elements)
            {
                TryAddElement(curElement);
            }
        }

        private void TryAddElement(MappingElement element)
        {
            if (loadedAssets.ContainsKey(element.ID))
            {
                DebugHelper.PrintFormatted(LogType.Warning, "{0} already contains key {1}.", GetType().Name, element.ID.ToString());
                return;
            };

            loadedAssets.Add(element.ID, element.Element);
        }
        #endregion
    }
}