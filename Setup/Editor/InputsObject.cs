using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Setup
{

    /// <summary>
    /// A scriptable object to hold the input axes. 
    ///   Using a custom object to transfer the axes over is 
    ///     more transparent than loading directly from a copy of InputManager.asset
    /// </summary>
    public class InputsObject : ScriptableObject
    {
        public SetupInputManager.InputAxis[] Axes;

        public static string SaveName => "InputAxes";

        public static class HoldThisCreateInputs
        {
            [MenuItem("Assets/Create/ArgonInternal/Export Input Axes From Current InputManager.asset")]
            public static void Create()
            {
                var inputsObject = ScriptableObject.CreateInstance<InputsObject>();

                inputsObject.Axes = SetupInputManager.ToInputAxes();

                AssetDatabase.CreateAsset(inputsObject, $"Assets/Argon/Setup/ExportImport/{InputsObject.SaveName}.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = inputsObject;
            }
        }

        public static InputsObject FindInProject()
        {
            return ShGameObjectHelper.FindInProject<InputsObject>(SaveName);
        }
    }
}
