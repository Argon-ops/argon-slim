using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DuksGames.Tools
{
    public interface IKeySetAccess
    {
        string AppendSuffix(string suffix);
        string TargetKey { get; }
        bool ContainsKey(string key);
    }

    public interface IProcessorKeySet
    {
        bool IsTargetKey(string key);
        IEnumerable<string> GetKeys();
        AbstractCustomPropProcessor CreateProcessor(CustomPropApplyInfo info);

        bool ExcludeObject(string key, GameObject go);
    }


    public abstract class AbstractCustomPropKeySet<T> : ScriptableObject, IProcessorKeySet, IKeySetAccess
         where T : AbstractCustomPropProcessor, new()
    {

        public virtual bool ContainsKey(string key)
        {
            foreach (string k in this.GetKeys())
            {
                if (k == key)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsTargetKey(string key)
        {
            return this.TargetKey == key;
        }

        public string AppendSuffix(string suffix)
        {
            return $"{this.TargetKey}{suffix}";
        }

        public abstract string TargetKey { get; }
        public abstract IEnumerable<string> GetKeys();


        public bool ExcludeObject(string key, GameObject go)
        {
            return this._ExcludeObject(key, go);
        }

        protected virtual bool _ExcludeObject(string key, GameObject go) { return false; }

        public AbstractCustomPropProcessor CreateProcessor(CustomPropApplyInfo info)
        {
            return AbstractCustomPropProcessor.CreateProcessor<T>(info, this);
        }
    }

    public abstract class AbstractCustomPropProcessor
    {
        #region accessors

        public T GetValueWithSuffix<T>(string suffix, bool logKeyNotFound = false, T fallback = default(T))
        {
            if (typeof(T) == typeof(bool))
            {
                return this.Config.getValue(this.KeySet.AppendSuffix(suffix));
            }
            return this.Config.getTypedValue<T>(this.KeySet.AppendSuffix(suffix), logKeyNotFound, fallback);
        }

        protected string GetStringWithSuffix(string suffix) { return this.GetValueWithSuffix<string>(suffix); }

        protected int GetIntWithSuffix(string suffix, int fallback = 0) { return this.GetIntWithSuffixLog(suffix, false, fallback); }

        protected int GetIntWithSuffixLog(string suffix, bool logKeyNotFound = false, int fallback = 0)
        {
            return this.GetValueWithSuffix<int>(suffix, logKeyNotFound, fallback);
        }

        protected bool GetBoolWithSuffix(string suffix, bool coerceKeyNotPresent = false)
        {
            try
            {
                return this.Config.getBool(this.AppendSuffix(suffix), coerceKeyNotPresent);
            }
            catch (KeyNotFoundException KeyNotFound)
            {
                Debug.LogWarning($" Key {suffix} Not Found. Did you forget to add the key to your implementation of AbstractCustomPropKeySet.GetKeys()? The target object is {this.ApplyInfo.Target.name}");
                throw KeyNotFound;
            }
        }

        protected float GetFloatWithSuffix(string suffix) { return this.GetValueWithSuffix<float>(suffix); }

        protected Vector3 GetVector3WithSuffix(string suffix, bool logNotFound = false, Vector3 fallback = new Vector3())
        {
            var floats = this.Config.getTypedValue<Vector3>(this.AppendSuffix(suffix), logNotFound, fallback);
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        protected Color GetColorWithSuffix(string suffix, bool logNotFound = false, Color fallback = new Color())
        {
            // Blender gives us Color_Gamma float arrays as strings. So we have to parse them manually.
            //   and yet this isn't true of blender position vectors which parse to Vector3 automagically 
            var strArray = this.Config.getTypedValue<string>(this.AppendSuffix(suffix));
            if (strArray == null) { return fallback; }

            strArray = strArray.Trim('[', ']', ' ');
            var f = strArray.Split(",").Select(str => float.Parse(str.Trim())).ToArray();

            if (f.Length < 3) { return fallback; }

            var c = new Color(f[0], f[1], f[2]);
            if (f.Length > 3)
            {
                c.a = f[3];
            }
            return c;
        }

        protected float[] GetFloatArrayWithSuffix(string suffix, bool logNotFound = false, float[] fallback = null)
        {
            return this.Config.getTypedValue<float[]>(this.AppendSuffix(suffix), logNotFound, fallback);
        }

        #endregion

        public static T CreateProcessor<T>(CustomPropApplyInfo info, AbstractCustomPropKeySet<T> keySet) where T : AbstractCustomPropProcessor, new()
        {
            return new T
            {
                ApplyInfo = info,
                KeySet = keySet
            };
        }

        protected AbstractCustomPropProcessor() { }
        protected string AppendSuffix(string suffix)
        {
            return this.KeySet.AppendSuffix(suffix);
        }

        protected CustomPropApplyInfo ApplyInfo;
        protected IKeySetAccess KeySet;
        protected PrimitiveStorage Config = new();

        public bool ClaimKey(string key, dynamic value)
        {
            if (this.KeySet.ContainsKey(key))
            {
                this.Config.setValue(key, value);
                return true;
            }
            return false;
        }

    }

}