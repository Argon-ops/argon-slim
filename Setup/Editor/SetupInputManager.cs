using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Shared;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Animations;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DuksGames.Argon.Setup
{
    public class SetupInputManager
    {

        // credit: https://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html
        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        private static bool AxisDefined(string axisName)
        {
            return GetAxis(axisName) != null;
        }

        private static SerializedProperty GetAxis(string axisName)
        {
            SerializedObject serializedInputManager = SetupInputManager.GetCurrentInputManager();
            SerializedProperty axesProperty = serializedInputManager.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                if (!axis.Next(true))
                    break;
                if (axis.stringValue == axisName) return axis;
            }
            return null; 
        }

        static SerializedObject GetCurrentInputManager()
        {
            var im = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]; // AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object));
            UnityEngine.Assertions.Assert.IsFalse(im == null, "ARGON: No InputManager was found in ProjectSettings. ");
            return new SerializedObject(im);
        }


        [System.Serializable]
        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        // public enum FakeAxisWorkaroundEnum 
        // {
        //     // Hopefully we don't really need this. We are cherry picking a hard to diagnose problem
        //     //  where the UI in later versions of unity errors when trying to display our ScriptableObject
        //     X_Axis, Y_Axis, _3_Axis, _4_Axis, _5_Axis, _6_Axis, _7_Axis, _8_Axis, _9_Axis, _10_Axis, _11_Axis, _12_Axis, _13_Axis, _14_Axis, _15_Axis, _16_Axis, _17_Axis, _18_Axis, _19_Axis, _20_Axis, _21_Axis, _22_Axis, _23_Axis,  _24_Axis, _25_Axis, _26_Axis, _27_Axis, _28_Axis
        // }

        // public enum FakeJoyNumWorkaroundEnum 
        // {
        //     // Hopefully we don't really need this. We are cherry picking a hard to diagnose problem
        //     //  where the UI in later versions of unity errors when trying to display our ScriptableObject
        //     Get_Motion_From_All_Joysticks, Joystick_1, Joystick_2, Joystick_3, Joystick_4, Joystick_5, Joystick_6, Joystick_7, Joystick_8, Joystick_9, Joystick_10, Joystick_11, Joystick_12, Joystick_13, Joystick_14, Joystick_15, Joystick_16
        // }

        [System.Serializable]
        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            // public AxisType type;
            public int type;

            // public FakeAxisWorkaroundEnum axis;
            public int axis;
            // public FakeJoyNumWorkaroundEnum joyNum;
            public int joyNum;
        }

        static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) {
                return;
            } 

            SerializedObject serializedObject = SetupInputManager.GetCurrentInputManager(); 
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = (int)axis.axis - 1; // TODO: there must be some reason for - 1 but what could it be.
            GetChildProperty(axisProperty, "joyNum").intValue = (int)axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }

        public static InputAxis[] ToInputAxes()
        {
            SerializedObject serializedObject = SetupInputManager.GetCurrentInputManager(); 
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
;
            var list = new List<InputAxis>();
            foreach(var axisProperty in axesProperty) 
            {
                list.Add(FromAxis((SerializedProperty)axisProperty));
            }
            return list.ToArray();
        }

        static InputAxis FromAxis(SerializedProperty axisProperty) 
        {
            var x = new InputAxis();

            x.name = GetChildProperty(axisProperty, "m_Name").stringValue; 
            x.descriptiveName = GetChildProperty(axisProperty, "descriptiveName").stringValue; 
            x.descriptiveNegativeName = GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue; 
            x.negativeButton = GetChildProperty(axisProperty, "negativeButton").stringValue; 
            x.positiveButton = GetChildProperty(axisProperty, "positiveButton").stringValue; 
            x.altNegativeButton = GetChildProperty(axisProperty, "altNegativeButton").stringValue; 
            x.altPositiveButton =  GetChildProperty(axisProperty, "altPositiveButton").stringValue ; 
            x.gravity =  GetChildProperty(axisProperty, "gravity").floatValue; 
            x.dead =  GetChildProperty(axisProperty, "dead").floatValue; 
            x.sensitivity = GetChildProperty(axisProperty, "sensitivity").floatValue; 
            x.snap = GetChildProperty(axisProperty, "snap").boolValue; 
            x.invert = GetChildProperty(axisProperty, "invert").boolValue; 
            x.type = GetChildProperty(axisProperty, "type").intValue;
            // x.type = (AxisType)GetChildProperty(axisProperty, "type").intValue;
            x.axis = GetChildProperty(axisProperty, "axis").intValue + 1; 
            // x.axis = (FakeAxisWorkaroundEnum)GetChildProperty(axisProperty, "axis").intValue + 1; 
            x.joyNum = GetChildProperty(axisProperty, "joyNum").intValue; 
            // x.joyNum = (FakeJoyNumWorkaroundEnum)GetChildProperty(axisProperty, "joyNum").intValue; 

            return x;
        }

        [MenuItem("Tools/Argon/Demo Scene/Restore Demo Scene Input Manager")]
        static void RestoreInputs()
        {
            var inputsObject = InputsObject.FindInProject();
            foreach(var axis in inputsObject.Axes)
            {
                SetupInputManager.AddAxis(axis);
            }
        }

        [MenuItem("Tools/Argon/Debug: Dump InputManager")]
        static void DumpInputManager()
        {
            var iaxes = ToInputAxes();
            foreach(var ax in iaxes) {
                Debug.Log($"ax: {ax.name}");
            }

            SerializedObject serializedObject = SetupInputManager.GetCurrentInputManager(); 
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            foreach (var axis in axesProperty) 
            {
                var axp = GetChildProperty((SerializedProperty)axis, "axis");
                Debug.Log($"AX {axp} val? {axp.enumValueFlag}");
                break;
            }
        }

    }
}