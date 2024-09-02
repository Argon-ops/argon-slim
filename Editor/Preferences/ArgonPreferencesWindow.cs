using UnityEditor;
using UnityEngine;

namespace DuksGames.Tools
{
    public class ArgonPreferencesWindow : EditorWindow
    {
        private ArgonImportPreferences preferences;

        [MenuItem("Window/Argon/Preferences")]
        public static void ShowWindow()
        {
            GetWindow<ArgonPreferencesWindow>("Argon");
        }

        private void OnEnable()
        {
            this.preferences = StoreArgonPreferences.GetOrCreatePreferences();
        }

        private void OnGUI()
        {
            GUILayout.Label("User Preferences", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            this.preferences.IsArgonEnabled = EditorGUILayout.Toggle("Is Argon Enabled", this.preferences.IsArgonEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                // Save the changes made to the ScriptableObject
                EditorUtility.SetDirty(this.preferences);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
