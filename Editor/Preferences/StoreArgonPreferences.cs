using UnityEditor;
using UnityEngine;

/*
FIX ME: don't call during ScriptedImporters or PostProcessors. So just 
   insist that the user create the preferences...?


 to "AssetDatabase.CreateAsset" are restricted during asset importing. AssetDatabase.CreateAsset() was called as part of running an import. 
 Please make sure this function is not called from ScriptedImporters or 
 PostProcessors, as it is a source of non-determinism and will be disallowed in a forthcoming release.
UnityEngine.StackTraceUtility:ExtractStackTrace ()
DuksGames.Tools.StoreArgonPreferences:GetOrCreatePreferences () (at Assets/argon-slim/Editor/Preferences/StoreArgonPreferences.cs:26)
DuksGames.Tools.ArgonCachedPreferences:GetOrCreateCachedPreferences () (at Assets/argon-slim/Editor/Preferences/ArgonCachedPreferences.cs:23)
DuksGames.Tools.ArgonCachedPreferences:get_Preferences () (at Assets/argon-slim/Editor/Preferences/ArgonCachedPreferences.cs:15)
DuksGames.Tools.Logger:ImportLog (string) (at Assets/argon-slim/Editor/Logger/Logger.cs:11)
DuksGames.Tools.PostProcessorRouter:OnPostprocessGameObjectWithUserProperties (UnityEngine.GameObject,string[],object[]) (at Assets/argon-slim/Editor/CustomPropProcessor/PostProcessorRouter.cs:86)
UnityEditor.AssetPostprocessingInternal:PostprocessGameObjectWithUserProperties (UnityEngine.GameObject,string[],object[])

*/

namespace DuksGames.Tools
{
    public static class StoreArgonPreferences
    {
        // public static ArgonImportPreferences GetPreferences() 
        // {
        //     return MelGameObjectHelper.FindInProject<ArgonImportPreferences>("ArgonImportPreferences", "ArgonImportPreferences");
        // }

        // public static ArgonImportPreferences GetOrCreatePreferences() 
        // {
        //     var preferences = StoreArgonPreferences.GetPreferences();
            
        //     if (preferences == null)
        //     {
        //         preferences = ScriptableObject.CreateInstance<ArgonImportPreferences>();

        //         // if not exists
        //         if(!AssetDatabase.IsValidFolder("Assets/Argon")) 
        //         {
        //             AssetDatabase.CreateFolder("Assets", "Argon");
        //         }
        //         AssetDatabase.CreateAsset(preferences, "Assets/Argon/ArgonImportPreferences.asset");

        //     }
        //     return preferences;
        // }

        public static bool IsArgonEnabled() 
        {
            // We need to overhaul Argon Preferences and not use a ScriptableObject to store them
            //  for now just return a value
            return true;

            // var preferences = StoreArgonPreferences.GetPreferences();
            // if (preferences == null) 
            // {
            //     return true;  
            // }
            // return preferences.IsArgonEnabled;
        }
    }
}