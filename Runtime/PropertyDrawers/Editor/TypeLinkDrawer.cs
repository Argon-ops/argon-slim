using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.PropertyDrawers
{
    /// <summary>
    /// Drawer for the TypeEnforce attribute that forces the user to pick
    /// a component of the provided type. Facilitates exposing interface types 
    /// in the inspector. 
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeEnforceAttribute))]
    public class TypeLinkDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var link = attribute as TypeEnforceAttribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.BeginChangeCheck();


                var ob = EditorGUI.ObjectField(
                    position,
                    label,
                    property.objectReferenceValue,
                    typeof(Component), // can't use link.Type. Editor can't search for interfaces 
                    link.AllowSceneObjects);

                if (EditorGUI.EndChangeCheck())
                {
                    if (ob != null)
                    {
                        if (ob is Component component)
                        {
                            // TODO: make sure that this doesn't break serializability
                            //   when link.Type is an interface. 
                            ob = component.GetComponent(link.Type);
                        }
                        else if (ob is GameObject gameObject)
                        {
                            ob = gameObject.GetComponent(link.Type);
                        }
                        else
                        {
                            ob = null;
                        }

                        if (ob == null)
                        {
                            Debug.LogWarning($"This property requires an object with a component of type {link.Type.Name} attached to it");
                        }
                    }

                    // hacky way to let type enforce support enforcing types on GameObject properties
                    if(ob != null && property.type.Contains("GameObject")) 
                    {
                        property.objectReferenceValue = ((Component) ob).gameObject;
                        return;
                    }

                    property.objectReferenceValue = ob;
                }

            }
        }
    }
}