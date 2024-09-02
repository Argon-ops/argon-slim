using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using System.IO;
using DuksGames.Argon.Shared;
using System.Reflection;
using UnityEngine.UIElements;

namespace DuksGames.Argon.Shared
{

    public class HierarchySearchException : System.Exception
    {
        public HierarchySearchException() : base() { }
        public HierarchySearchException(string s) : base(s) { }
        public HierarchySearchException(string s, System.Exception inner) : base(s, inner) { }

    }

    public static class ShGameObjectHelper
    {
        public static T AddIfNotPresent<T>(GameObject go) where T : Component
        {
            if (go.GetComponent<T>() != null) { return go.GetComponent<T>(); }
            return go.AddComponent<T>();
        }

        public static string GetUnusedChildName(string name, GameObject parent)
        {
            var existing = parent != null ? parent.transform.ImmediateChildren().Select(t => t.name).ToArray() :
                SceneManager.GetActiveScene().GetRootGameObjects().Select(g => g.name).ToArray();
            return ObjectNames.GetUniqueName(existing, name);
        }

        public static GameObject CreatePrimitiveAt(Vector3 global, GameObject parent, PrimitiveType primitiveType = PrimitiveType.Cube, string nameSuggestion = "")
        {
            nameSuggestion = nameSuggestion.Length == 0 ? $"{parent.name}-c" : nameSuggestion;
            var go = GameObject.CreatePrimitive(primitiveType);
            go.name = ShGameObjectHelper.GetUnusedChildName(nameSuggestion, parent);
            go.transform.position = global;
            go.transform.SetParent(parent.transform);
            return go;
        }

        public static GameObject CreateNewAt(Vector3 global, GameObject parent, string nameSuggestion = "")
        {
            nameSuggestion = nameSuggestion.Length == 0 ? $"{parent.name}-c" : nameSuggestion;
            var go = new GameObject(ShGameObjectHelper.GetUnusedChildName(nameSuggestion, parent));
            go.transform.position = global;
            go.transform.SetParent(parent.transform);
            return go;
        }


        /// <summary>
        /// Gets a game object if it already exists as a direct child of 'parent'.
        /// If parent is null, searches the top level game objects.
        /// Creates the game object as a child of 'parent' if none exists. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject GetChildOfOrCreate(string name, Transform parent = null)
        {
            var children = parent != null ?
                parent.ImmediateChildren().Select(ch => ch.gameObject) :
                SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            return go;
        }

        public static GameObject CreateChild(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            return go;
        }

        public static int ParseInt(System.Object val)
        {
            if (val == null) { return 0; }
            if (val is int || val is float) { return (int)val; }

            int result;
            if (int.TryParse((string)val, out result))
            {
                return result;
            }
            return -1;
        }

        public static int ParseAxis(System.Object val)
        {
            if (val is int || val is float)
            {
                return Mathf.Clamp((int)val, 0, 2);
            }
            if (val is string v)
            {
                if (string.IsNullOrEmpty(v)) { return 0; }
                int result = 0;
                if (int.TryParse(v, out result))
                {
                    return result;
                }
            }
            return 0;
        }

        public static Vector3 GetAxis(int axisIndex)
        {
            return axisIndex switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                _ => Vector3.forward,
            };
        }

        public static T FindInProjectOrWarn<T>(string searchName, string typeName = "", string failWarning = "") where T : Object
        {
            failWarning = failWarning.Length == 0 ? $"Search for {searchName} failed." : failWarning;
            return ShGameObjectHelper.FindInProject<T>(searchName, typeName, failWarning);
        }

        public static GameObject FindGameObjectInProjectEnforceType<T>(string searchName, string typeName = "", string failWarning = null)
        {
            var result = ShGameObjectHelper.FindInProject<GameObject>(searchName, typeName, failWarning);
            if (!result) { return null; }
            Assert.IsTrue(result.GetComponent<T>() != null, $"[{result}] doesn't have a component of type '{typeof(T)}'");
            return result;
        }

        public static T FindInProject<T>(
            string searchName,
            string typeName = "",
            string failWarning = null) where T : Object
        {
            return ShGameObjectHelper.FindInProject<T>(searchName, "", false, typeName, failWarning);
        }

        //https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        public static T FindInProject<T>(
            string searchName,
            string preferExtension,
            bool requireExtension,
            string typeName = "",
            string failWarning = null) where T : Object
        {
            var assetPaths = ShGameObjectHelper.FindAssetPaths(searchName, typeName, false, true, failWarning);

            if (assetPaths == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(preferExtension) && !preferExtension.StartsWith("."))
            {
                preferExtension = $".{preferExtension}";
            }

            var ordered = assetPaths.OrderBy(s =>
            {
                if (string.IsNullOrEmpty(preferExtension)) return 0;
                return Path.GetExtension(s).ToLower() == preferExtension.ToLower() ? 0 : 1;
            });
            foreach (var assetPath in ordered)
            {
                var found = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                if (requireExtension && Path.GetExtension(assetPath).ToLower() != preferExtension.ToLower()) { break; }
                if (found) { return found; }
            }
            return null;

        }

        public static IEnumerable<string> FindAssetPaths(
            string searchName,
            string typeName = "",
            bool matchPathNameOrFilename = true,
            bool wholeFilenameMatch = true,
            string failWarning = null)
        {
            if (searchName == null || searchName.Length == 0) { return null; }

            string typeSearch = typeName.Length > 0 ? $"t:{typeName}" : "";
            var guids = AssetDatabase.FindAssets($"{searchName} {typeSearch}", null);

            if (guids.Length == 0)
            {
                if (failWarning != null)
                    Debug.LogWarning($"Asset search for {searchName} failed: {failWarning}");
                return null;
            }

            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            if (matchPathNameOrFilename)
            {
                return paths;
            }
            return paths.Where(p =>
            {
                if (wholeFilenameMatch)
                {
                    return Path.GetFileNameWithoutExtension(p) == searchName ||
                        Path.GetFileName(p) == searchName;
                }
                return Path.GetFileName(p).Contains(searchName);
            });
        }

        public static GameObject FindInScene(string name)
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var found = root.transform.FindRecursive(name);
                if (found)
                {
                    return found.gameObject;
                }
            }
            return null;
        }

        public static T FindInScene<T>()
        {
            var result = ShGameObjectHelper.FindInScene(trans => trans.GetComponent<T>() != null);
            return result != null ? result.GetComponent<T>() : default(T);
        }
        
        public static GameObject FindInScene(System.Func<Transform, bool> condition) 
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var found = root.transform.FindRecursive(condition);
                if (found)
                {
                    return found.gameObject;
                }
            }
            return null;
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

        #if UNITY_EDITOR

        public static UnityEngine.Object GetFieldValue(Component fieldOwner, string fieldName)
        {
            var ser = new SerializedObject(fieldOwner);
            var prop = ser.FindProperty(fieldName);
            
            if(prop.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning($"Expected an objectReference property type. Instead got this type: '{prop.propertyType}'");
                return null;
            }

            return prop.objectReferenceValue;
        }

        public static void AssignObjectReferenceField(Component fieldOwner, string fieldName, UnityEngine.Object assigneeOwner, int index = -1) 
        {
            var finfo = fieldOwner.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if(finfo.FieldType.IsArray)
            {
                ShGameObjectHelper._AssignObjectReferenceAtArrayIndex(fieldOwner, fieldName, assigneeOwner, index);
                return;
            }

            ShGameObjectHelper._AssignObjectReferenceAtField(fieldOwner, fieldName, assigneeOwner);
        }

        static void _AssignObjectReferenceAtField(Component fieldOwner, string fieldName, UnityEngine.Object assigneeOwner)
        {
            
            var ser = new SerializedObject(fieldOwner);
            var property = ser.FindProperty(fieldName);
            
            if(property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning($"Expected an objectReference property type. Instead got this type:  '{property.propertyType}'. " + 
                                $"For fieldOwner:  '{fieldOwner.name}' type: '{fieldOwner.GetType().Name}' |. fieldName: '{fieldName}'");
                return;
            }

            Debug.Log($"Property: {property.name} : isArray: {property.isArray} : fieldName: {fieldName} ");
            var finfo = fieldOwner.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var type = finfo.FieldType;
            ShGameObjectHelper._ApplyToObjectReferenceProperty(property, type, assigneeOwner);
            ser.ApplyModifiedProperties();
        }

        static void _AssignObjectReferenceAtArrayIndex(Component fieldOwner, string fieldName, UnityEngine.Object assigneeOwner, int index)
        {
            var ser = new SerializedObject(fieldOwner);
            var finfo = fieldOwner.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            var propertyAtIndex = ser.FindProperty($"{fieldName}.Array.data[{index}]");
            var type = finfo.FieldType.GetElementType();
            Debug.Log($"Get Elem type: '{finfo.FieldType.GetElementType().Name}' fieldName: {fieldName}");
            ShGameObjectHelper._ApplyToObjectReferenceProperty(propertyAtIndex, type, assigneeOwner);
            ser.ApplyModifiedProperties();
        }

        static void _ApplyToObjectReferenceProperty(SerializedProperty property, System.Type type, UnityEngine.Object assigneeOwner)
        {
            if(type == typeof(GameObject)) 
            {
                Debug.Log($"will assign GO: '{assigneeOwner.name}");
                property.objectReferenceValue = assigneeOwner;
                return;
            }

            if(type.IsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.Log($"was SO type: {type.Name}");
                property.objectReferenceValue = assigneeOwner;
                return;
            }

            if(assigneeOwner is GameObject go)
            {
                property.objectReferenceValue = go.GetComponent(type);
                return;
            } 
            if (assigneeOwner is Component  co)
            {
                property.objectReferenceValue = co.GetComponent(type);
                return;
            }
            else 
            {
                throw new System.Exception($"Assignee owner was a type we don't know how to handle: {assigneeOwner.GetType().Name}");
            }
        }


        #endif

        // public static Transform FindInRootOrThrow(Transform child, string objectName, ImportHierarchyLookup importHierarchyLookup) 
        // {
        //     var found = ShGameObjectHelper.FindInRoot(child, objectName, importHierarchyLookup);
        //     if (found) {
        //         return found;
        //     }
        //     throw new HierarchySearchException($"No object {objectName} found in the hierarchy of {child.name}. Root transform {child.root.name}. | root instanceId: {child.root.gameObject.GetInstanceID()} also failed IHL: {importHierarchyLookup.DDump()}");
        // }

        public static Transform FindInRootOrThrow(Transform child, string objectName)
        {
            var found = ShGameObjectHelper.FindInRoot(child, objectName);
            if (found == null)
            {
                throw new HierarchySearchException($"No object {objectName} found in the hierarchy of {child.name} | root instanceId: {child.root.gameObject.GetInstanceID()}");
            }
            return found;
        }

        public static IEnumerable<GameObject> GetSiblings(Transform t)
        {
            if (t.parent == null) {
                foreach(GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    yield return root;
                }
            } 
            else 
            {
                for(int i=0; i<t.parent.childCount; ++i)
                {
                    yield return t.parent.GetChild(i).gameObject;
                }
            }
        }

        public static IEnumerable<GameObject> GetDirectChildrenOrRootGameObjects(Transform t)
        {
            if (t == null) {
                foreach(GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    yield return root;
                }
            } 
            else 
            {
                for(int i=0; i<t.childCount; ++i)
                {
                    yield return t.GetChild(i).gameObject;
                }
            }
        }


        // public static Transform FindInRoot(Transform child, string objectName, ImportHierarchyLookup importHierarchyLookup) 
        // {
        //     if (string.IsNullOrEmpty(objectName)) {
        //         return null;
        //     }

        //     var found = ShGameObjectHelper.FindInRoot(child, objectName);
        //     if(found) {
        //         return found;
        //     }

        //     if (importHierarchyLookup.TryGetInstanceId(objectName, out int instanceId)) {
        //         found = child.root.FindRecursiveSelfInclusive(t => t.gameObject.GetInstanceID() == instanceId);
        //         if(found) {
        //             return found;
        //         }
        //     }

        //     return null;
        // }

        public static Transform FindInRoot(Transform child, string objectName)
        {
            return child.root.FindRecursive(objectName);
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

        static Transform FindCommon(Transform a, Transform b)
        {
            var aLineage = a.ParentsInclusive().Reverse();
            var bLineage = b.ParentsInclusive().Reverse();
            Transform lastCommonParent = null;
            foreach (Transform commonParent in aLineage.Zip(bLineage, (ppA, ppB) => ppA == ppB ? ppA : null))
            {
                if (commonParent)
                {
                    lastCommonParent = commonParent;
                    continue;
                }
                return lastCommonParent;
            }
            return lastCommonParent;
        }

        public static Transform CommonParent(params Transform[] transforms)
        {
            if (transforms.Length == 0) { return null; }
            if (transforms.Length == 1) { return transforms[0]; }

            Transform result = ShGameObjectHelper.FindCommon(transforms[0], transforms[1]);
            for (int i = 2; i < transforms.Length; ++i)
            {
                result = ShGameObjectHelper.FindCommon(transforms[i], result);
                if (!result)
                {
                    return null;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the first UIDocument found or, failing that, the first canvas found. Throws if neither is found
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Component FindAnyUIRoot()
        {
            var uidoc = GameObject.FindObjectOfType<UIDocument>();
            if (uidoc) 
                return uidoc; 

            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas) 
                return canvas;

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Checks if the provided tag exists using case-insensitve matching. If there's a match
        ///   returns the matched tag. If no match, adds the tag string and returns the tag string.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string AddTag(string tag)
        {
            if (Application.isPlaying) { throw new System.Exception("Editor scripts only"); }

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            for (int i = 0; i < tagsProp.arraySize; ++i)
            {

                if (tagsProp.GetArrayElementAtIndex(i).stringValue.ToLower() == tag.ToLower())
                {
                    return tagsProp.GetArrayElementAtIndex(i).stringValue;
                }
            }

            tagsProp.InsertArrayElementAtIndex(0);
            var n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
            tagManager.ApplyModifiedProperties();
            tagManager.Update();

            return tag;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Add a layer at the first empty layer slot, if no matching layer exists.
        ///  Case-insensitive match check; so you'll want to use the return value not the parameter.
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns>The name of the layer.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string AddLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                return null;
            }

            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject serializedObject = new SerializedObject(asset[0]);
                SerializedProperty layers = serializedObject.FindProperty("layers");

                for (int i = 0; i < layers.arraySize; ++i)
                {
                    if (layers.GetArrayElementAtIndex(i).stringValue.ToLower() == layerName.ToLower())
                    {
                        return layers.GetArrayElementAtIndex(i).stringValue;     // Layer already present
                    }
                }


                for (int i = 0; i < 32; i++)
                {
                    // Extend layers if necessary
                    if (i >= layers.arraySize)
                        layers.arraySize = i + 1;

                    if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                    {
                        layers.GetArrayElementAtIndex(i).stringValue = layerName;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                        if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                        {
                            return layerName;     // to avoid unity locked layer
                        }
                    }
                }
                throw new System.Exception($"Maximum (32) number of layers already added. Can't add {layerName}");
            }
            throw new System.Exception($"You'll never see this exception");
        }
#endif


    }
}