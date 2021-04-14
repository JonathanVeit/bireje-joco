using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Can be used to display lists in the UI
    /// Will dynamically instantiate and destroy the items on the given grid
    /// </summary>
    [Serializable]
    public class UIList <TItem> where TItem : Component
    {
        [SerializeField] private Transform grid;
        [SerializeField] private TItem prefab;

        private Dictionary<TItem, GameObject> items = new Dictionary<TItem, GameObject>();
        private List<TItem> itemsList = new List<TItem>();

        #region public Member
        public int Count { get { return itemsList.Count; } }
        #endregion

        // add
        public TItem Add() 
        {
            var newItem = UnityEngine.Object.Instantiate(prefab, grid);

            ForceLayoutUpdate();
            items.Add(newItem, newItem.gameObject);
            itemsList.Add(newItem);
            return newItem;
        }

        public TItem[] AddItems(int amount)
        {
            var itemList = new List<TItem>();
            for (int i = 0; i < amount; i++)
            {
                var newItem = UnityEngine.Object.Instantiate(prefab, grid);
                var comp = newItem.GetComponent<TItem>();

                ForceLayoutUpdate();
                items.Add(comp, newItem.gameObject);
                itemsList.Add(comp);
                itemList.Add(comp);
            }

            return itemList.ToArray();
        }

        // get 
        public TItem this[int i]
        {
            get
            {
                return itemsList[i];
            }
        }

        public GameObject GetItemGameObject(TItem item)
        {
            return items[item];
        }

        // remove 
        public void RemoveAt(int index)
        {
            UnityEngine.Object.Destroy(items[itemsList[index]]);
            items.Remove(itemsList[index]);
            itemsList.RemoveAt(index);
            ForceLayoutUpdate();
        }

        public void Remove(TItem item)
        {
            UnityEngine.Object.Destroy(items[item]);
            items.Remove(item);
            itemsList.Remove(item);
            ForceLayoutUpdate();
        }

        public void Clear()
        {
            foreach (Transform curChild in grid)
                UnityEngine.Object.Destroy(curChild.gameObject);

            items.Clear();
            itemsList.Clear();
            ForceLayoutUpdate();
        }

        #region Helper
        private void ForceLayoutUpdate() 
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());
        }
        #endregion
    }
}