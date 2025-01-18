using UnityEditor;
using UnityEngine;

namespace DuksGames.Tools
{
    // public class ArgonPreferencesWindow : EditorWindow
    // {
    //     private ArgonImportPreferences preferences => ArgonCachedPreferences.Preferences;

    //     [MenuItem("Window/Argon/Preferences")]
    //     public static void ShowWindow()
    //     {
    //         GetWindow<ArgonPreferencesWindow>("Argon");
    //     }

    //     private void OnEnable()
    //     {
    //         ArgonCachedPreferences.GetOrCreateCachedPreferences();
    //         // this.preferences = StoreArgonPreferences.GetOrCreatePreferences();
    //     }

    //     private void OnGUI()
    //     {
    //         GUILayout.Label("User Preferences", EditorStyles.boldLabel);

    //         EditorGUI.BeginChangeCheck();

    //         this.preferences.IsArgonEnabled = EditorGUILayout.Toggle("Is Argon Enabled", this.preferences.IsArgonEnabled);
    //         this.preferences.IsImportLoggerEnabled = EditorGUILayout.Toggle("Is Import Logger Enabled", this.preferences.IsImportLoggerEnabled);

    //         if (EditorGUI.EndChangeCheck())
    //         {
    //             // Save the changes made to the ScriptableObject
    //             EditorUtility.SetDirty(this.preferences);
    //             AssetDatabase.SaveAssets();
    //         }
    //     }
    // }
}
