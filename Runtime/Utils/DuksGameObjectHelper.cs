using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DuksGames.Argon.Utils
{
    public static class DuksGameObjectHelper
    {
        //https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        public static T FindInProject<T>(string searchName, string typeName = "", string failWarning = null) where T : Object
        {
            if (searchName == null || searchName.Length == 0)
            {
                return null;
            }
            string typeSearch = typeName.Length > 0 ? $"t:{typeName}" : "";
            var guids = AssetDatabase.FindAssets($"{searchName} {typeSearch}", null);

            if (guids.Length == 0)
            {
                if (failWarning != null)
                    Debug.LogWarning($"Asset search for {searchName} failed: {failWarning}");
                return null;
            }

            var result = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(T));
            return result;
        }

        public static T GetOrAdd<T>(GameObject go) where T : Component
        {
            if (go.GetComponent<T>() != null) { return go.GetComponent<T>(); }
            return go.AddComponent<T>();
        }

        public static System.Type FindType(string typeName, string namespaceString = "")
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetTypes().FirstOrDefault(t =>
                {
                    return t.Name == typeName && (string.IsNullOrEmpty(namespaceString) || namespaceString == t.Namespace);
                });

                if (type != null) { return type; }
            }
            return null;
        }

    }
}