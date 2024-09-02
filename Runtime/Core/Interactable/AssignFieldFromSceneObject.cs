using System;
using System.Linq;
using System.Text.RegularExpressions;
using Codice.CM.Common;
using DuksGames.Argon.Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

namespace DuksGames.Argon.Core
{
    [CustomEditor(typeof(AssignFieldFromSceneObject)), CanEditMultipleObjects]
    public class AssignFieldFromSceneObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            if(GUILayout.Button("Rewire this object"))
            {
                ((AssignFieldFromSceneObject)target).Assign();
            }

            if(GUILayout.Button("Rewire All"))
            {
                AssignFieldFromSceneObject.Rewire();
            }

            base.OnInspectorGUI();
        }
    }
    public class AssignFieldFromSceneObject : MonoBehaviour
    {
        public Component FieldOwner;
        public string FieldName;
        public string TargetObjectInScene;

        void Awake()
        {
            this.AssignIfNull();
            Destroy(this);
        }

        void AssignIfNull()
        {
            if (ShGameObjectHelper.GetFieldValue(this.FieldOwner, this.FieldName) == null)
            {
                this.Assign();
            }
        }


        [MenuItem("Mel/Rewire all AssignFieldFromSceneObject connections")]
        public static void Rewire()
        {
            foreach(var assignField in GameObject.FindObjectsOfType<AssignFieldFromSceneObject>())
            {
                assignField.Assign();
            }
        }

        public void Assign()
        {
            var target = ShGameObjectHelper.FindInScene(this.TargetObjectInScene);
            ShGameObjectHelper.AssignObjectReferenceField(this.FieldOwner, this.FieldName, target);
        }

    }
}