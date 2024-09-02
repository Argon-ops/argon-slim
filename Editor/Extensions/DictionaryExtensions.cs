using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public static class DictionaryExtensions
    {
        public static void AddOrOverwrite<K,V>(this Dictionary<K,V> d, K key, V value) {
            if (d.ContainsKey(key)) {
                d[key] = value;
                return;
            }
            d.Add(key, value);
        }
    }
}