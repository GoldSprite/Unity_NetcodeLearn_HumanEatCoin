using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoldSprite.AutoListUI
{
    public class AutoListView : MonoBehaviour
    {
        public GameObject ItemPrefab;
        public Transform ContentTrans;
        private Dictionary<object, AutoListItem> items = new();

        private void Update()
        {
        }

        [ContextMenu("AddAItem")]
        public void AddItem()
        {
            AddItem("AA");
        }
        [ContextMenu("RemoveAItem")]
        public void RemoveItem()
        {
            RemoveItem("AA");
        }
        [ContextMenu("GetItem")]
        public void GetItemm()
        {
            var i = GetItem<string, AutoListItem>("AA");
        }
        [ContextMenu("FixItem")]
        public void FixItem()
        {
            var i = GetItem<string, AutoListItem>("AA");
            i.Name = "AAAAAAAA";
            UpdateItemContent("AA");
        }

        public bool AddItem<T>(T key)
        {
            if (items.ContainsKey(key)) return false;

            var obj = Instantiate(ItemPrefab, ContentTrans);
            var aItem = obj.GetComponent<AutoListItem>();

            obj.SetActive(true);
            items.Add(key, aItem);
            UpdateItemContent(key);
            return true;
        }

        public bool RemoveItem<T>(T key)
        {
            if (!items.ContainsKey(key)) return false;

            Destroy(items[key].gameObject);
            items.Remove(key);
            return true;
        }

        public V GetItem<T, V>(T key) where V : AutoListItem
        {
            if (!items.ContainsKey(key)) return default(V);

            return (V)items[key];
        }

        public bool UpdateItemContent<T>(T key)
        {
            if (!items.ContainsKey(key)) return false;

            items[key].UpdateContent();
            return true;
        }

        public IEnumerator GetEnumerator() { return items.GetEnumerator(); }
        public Dictionary<object, AutoListItem> GetItems() { return items; }
    }
}
