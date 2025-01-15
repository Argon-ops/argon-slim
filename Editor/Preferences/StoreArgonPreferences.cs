using UnityEditor;
using UnityEngine;

namespace DuksGames.Tools
{
    public static class StoreArgonPreferences
    {
        public static ArgonImportPreferences GetPreferences() 
        {
            return MelGameObjectHelper.FindInProject<ArgonImportPreferences>("ArgonImportPreferences", "ArgonImportPreferences");
        }

        public static ArgonImportPreferences GetOrCreatePreferences() 
        {
            var preferences = StoreArgonPreferences.GetPreferences();
            
            if (preferences == null)
            {
                preferences = ScriptableObject.CreateInstance<ArgonImportPreferences>();

                // if not exists
                if(!AssetDatabase.IsValidFolder("Assets/Argon")) 
                {
                    AssetDatabase.CreateFolder("Assets", "Argon");
                }
                AssetDatabase.CreateAsset(preferences, "Assets/Argon/ArgonImportPreferences.asset");

            }
            return preferences;
        }

        public static bool IsArgonEnabled() 
        {
            var preferences = StoreArgonPreferences.GetPreferences();
            if (preferences == null) 
            {
                return true;  
            }

            return preferences.IsArgonEnabled;
        }
    }
}