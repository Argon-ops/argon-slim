using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using DuksGames.Argon.Shared;
using System;
using DuksGames.Argon.Animate;
using System.Linq;

namespace DuksGames.Argon.DemoScene
{

    [CustomEditor(typeof(NumericInputController))]
    public class NumericInputController_Inspector : Editor
    {
        public enum NumericGridType
        {
            SecurityKeypad
        }
        [System.Serializable]
        public class NumericGridLayoutInfo
        {
            public string Labels;
        }

        static NumericGridLayoutInfo GetInfo(NumericGridType numericGridType)
        {
            switch (numericGridType)
            {
                case NumericGridType.SecurityKeypad:
                default:
                    return new NumericGridLayoutInfo()
                    {
                        Labels = "123456789<0#"
                    };
            }
        }

        string ButtonRendererBaseName = "Button_";
        string PlayableClipWrapperBaseName = "PCH__Button";

        GameObject _root;

        static bool _showFoldout;


        private void BuildButton(SerializedProperty b, char label)
        {
            var ren = this._root.transform.root.FindRecursiveSelfInclusive(t =>
            {
                return t.name.Contains(this.ButtonRendererBaseName)
                    && t.name.Contains($"{label}")
                    && t.GetComponent<Renderer>() != null;
            });

            if (!ren)
            {
                Debug.Log($"no renderer for {label} ");
                return;
            }

            var pcw = this._root.transform.root.FindRecursiveSelfInclusive(t =>
                            t.name.IndexOf(this.PlayableClipWrapperBaseName, StringComparison.InvariantCulture) >= 0 &&
                             t.name.Contains($"{label}") &&
                             t.GetComponent<PlayableClipWrapper>() != null);

            if (!pcw)
            {
                Debug.Log($"no pcw for {label}");
                return;
            }

            b.FindPropertyRelative("Press").objectReferenceValue = pcw.GetComponent<PlayableClipWrapper>();
            b.FindPropertyRelative("Renderer").objectReferenceValue = ren.GetComponent<Renderer>();
            b.FindPropertyRelative("value").stringValue = $"{label}";
        }

        void BuildButtons(NumericInputController numericInputController, NumericGridLayoutInfo numericGridLayoutInfo)
        {


            if (this._root == null)
            {
                Debug.LogWarning($"Please specify a Target object");
                return;
            }
            var cols = serializedObject.FindProperty("_columns").intValue;
            var rows = serializedObject.FindProperty("_rows").intValue;
            var labels = numericGridLayoutInfo.Labels.ToCharArray();

            var _buttons = serializedObject.FindProperty("_buttons");
            _buttons.ClearArray();
            _buttons.arraySize = rows * cols;

            for (int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < cols; ++c)
                {
                    int i = r * cols + c;
                    char ch = labels[i];
                    var b = _buttons.GetArrayElementAtIndex(i);
                    this.BuildButton(b, ch);
                }
            }

            serializedObject.ApplyModifiedProperties();

        }


        public NumericGridType GridType;

        public override void OnInspectorGUI()
        {
            _showFoldout = EditorGUILayout.Foldout(_showFoldout, "Build the buttons array ");
            if (_showFoldout)
            {
                this.GridType = (NumericGridType)EditorGUILayout.EnumPopup("Grid Type: ", this.GridType);
                EditorGUILayout.LabelField("Labels: " + GetInfo(this.GridType).Labels);

                this.ButtonRendererBaseName = EditorGUILayout.TextField("Button Renderer Base Name: ", this.ButtonRendererBaseName);
                this.PlayableClipWrapperBaseName = EditorGUILayout.TextField("Playable Clip Wrapper Base Name: ", this.PlayableClipWrapperBaseName);

                this._root = (GameObject)EditorGUILayout.ObjectField("Target", this._root, typeof(GameObject), true);

                if (this._root != null)
                {
                    var nic = this.target as NumericInputController;
                    if (GUILayout.Button("Build Keypad"))
                    {
                        Debug.Log($"OK build it");
                        this.BuildButtons(nic, GetInfo(this.GridType));
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}