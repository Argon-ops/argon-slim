using UnityEngine;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class PrimitiveStorage
    {
        public Dictionary<string, dynamic> lookup = new Dictionary<string, dynamic>();

        public void setValue(string key, dynamic value)
        {
            if (this.lookup.ContainsKey(key))
            {
                this.lookup[key] = value;
                return;
            }

            this.lookup.Add(key, value);
        }

        #region accessors
        public dynamic getValue(string key, bool coerceKeyNotPresent = false)
        {
            if (coerceKeyNotPresent && !this.lookup.ContainsKey(key))
            {
                return 0;
            }
            return this.lookup[key];
        }
        public T getTypedValue<T>(string key, bool logKeyNotFound = false, T fallback = default(T))
        {
            try
            {
                return this.lookup[key];
            }
            catch (KeyNotFoundException)
            {
                if (logKeyNotFound) { Debug.LogWarning($"KeyNotFound for {key}. returning default(T)"); }
                return fallback;
            }
        }
        public int getInt(string key)
        {
            return (int)this.lookup[key];
        }
        public bool getBool(string key, bool coerceKeyNotPresent = false)
        {
            var result = this.getValue(key, coerceKeyNotPresent);
            if (result is float)
            {
                return result > 0f;
            }
            if (result is int)
            {
                return result > 0;
            }
            if (result is bool)
            {
                return result;
            }
            throw new System.Exception($"unsupported type: {result.GetType()} for key: {key} ");
        }
        public float getFloat(string key)
        {
            return this.getValue(key);
        }
        public string getString(string key)
        {
            return this.lookup[key];
        }

        #endregion
    }
}