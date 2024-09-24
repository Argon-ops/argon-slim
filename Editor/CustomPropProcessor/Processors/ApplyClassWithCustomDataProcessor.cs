using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using System;
using DuksGames.Argon.Shared;
using UnityEditor;
using Unity.Plastic.Newtonsoft.Json;
using DuksGames.Argon.Core;
using Microsoft.CSharp.RuntimeBinder;
using Unity.Properties;

namespace DuksGames.Tools
{

    public class ApplyClassWithCustomDataKeySet : AbstractCustomPropKeySet<ApplyClassWithCustomDataProcessor>
    {
        public static class KeyConventions
        {
            public const string Prefix = "m3ldata_";
            public const string PayloadSuffix = "_payload";
            public const string ApplyClassSuffix = "_apply_class_name";


            public const string ConfigSuffix_NOT_IN_USE = "_config"; 
            // not in use because we don't want to make a separate configurable dynamic data mechanism on the blender python side
        }
        public override string TargetKey => "_zxsdfwefsdf_target_key_does_not_apply_to_us";

        public override bool IsTargetKey(string key)
        {
            // use the payload key as the target key.
            var result = key.StartsWith(KeyConventions.Prefix) &&
                key.EndsWith(KeyConventions.PayloadSuffix);

            if(result)
                Logger.ImportLog($"APPLY GOT TARGET KEY: {key} | {result}".Blue());

            return result;
        }

        public override IEnumerable<string> GetKeys()
        {
            yield return null;
        }

        public override bool ContainsKey(string key)
        {
            return key.StartsWith(KeyConventions.Prefix);
        }


    }


    public class ApplyClassWithCustomDataProcessor : AbstractCustomPropProcessor,
            IApplyCustomProperties, ILinkerProcess 
    {

        class CustomComponentLikeKeySet
        {
            public string PayloadKey;
            public string ApplyClassKey;
            public string ApplyClassNamespaceKey;
            public string ConfigDataKey_NOT_IN_USE;
        }

        IEnumerable<CustomComponentLikeKeySet> GetKeySets()
        {
            var payloadKeys = this.Config.lookup.Keys.Where(k => 
                    k.StartsWith(ApplyClassWithCustomDataKeySet.KeyConventions.Prefix)
                    && k.EndsWith(ApplyClassWithCustomDataKeySet.KeyConventions.PayloadSuffix));

            Logger.ImportLog($"PAYLOADS: {payloadKeys.Count()}");

            return payloadKeys.Select(k =>
            {
                var prefixLength = ApplyClassWithCustomDataKeySet.KeyConventions.Prefix.Length;
                var suffixLength = ApplyClassWithCustomDataKeySet.KeyConventions.PayloadSuffix.Length;

                var componentLikeName = k.Substring(prefixLength, k.Length - prefixLength - suffixLength);
                return new CustomComponentLikeKeySet
                {
                    PayloadKey = k,
                    ApplyClassKey = ApplyClassWithCustomDataKeySet.KeyConventions.Prefix + componentLikeName +
                                ApplyClassWithCustomDataKeySet.KeyConventions.ApplyClassSuffix,
                    ConfigDataKey_NOT_IN_USE = ApplyClassWithCustomDataKeySet.KeyConventions.Prefix + componentLikeName +
                                ApplyClassWithCustomDataKeySet.KeyConventions.ConfigSuffix_NOT_IN_USE,
                };
            });
            
        }


        public void Apply()
        {
            
            foreach(var keySet in this.GetKeySets())
            {
                if (string.IsNullOrEmpty(keySet.PayloadKey))
                    continue;

                if (!this.Config.lookup.ContainsKey(keySet.PayloadKey))
                    continue;

                var className = this.Config.lookup[keySet.ApplyClassKey];
                var namespaceString = !string.IsNullOrEmpty(keySet.ApplyClassNamespaceKey) 
                                        && this.Config.lookup.ContainsKey(keySet.ApplyClassNamespaceKey) ?
                                                this.Config.lookup[keySet.ApplyClassNamespaceKey] : "";

                System.Type type = MelGameObjectHelper.FindType(className, namespaceString);

                Assert.IsFalse(type == null, $"No type found for className: '{className}' , namespace '{namespaceString}'");

                var component = this.ApplyInfo.Target.AddComponent(type);

                this.ApplyProperties(keySet, component);

                this.MethodCall(component, type, "_ARGON_OnApply", this.GetImportTimeConfigData(keySet));
            }
        }

        object GetImportTimeConfigData(CustomComponentLikeKeySet keySet)
        {
            if(!this.Config.lookup.ContainsKey(keySet.ConfigDataKey_NOT_IN_USE))
            {
                // will always return null. this is fine.
                // reminder: we wanted to be able to pass custom data to classes that 
                //    implemented our setup methods _ARGON_OnApply and _ARGON_OnLink.
                //      So that the user could, for example, tell the train track nodes to link up in reverse order.
                //     
                //      trouble is simply the brittle way we facilitate ui-editable dynamic data
                //         on the blender side: in short, the 'payload key' is a global that's baked into everything.
                //           
                return null;
            }

            var configstr = (string)this.Config.lookup[keySet.ConfigDataKey_NOT_IN_USE];
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(configstr);
        }

        // TODO: add ImportConfig data on the ble side
        //  and then update the methods so that they have a parameter to accept that data.
        ///    for not parameters are not used. 
        void MethodCall(Component component, Type type, string methodName, params object[] parameters)
        {
            var methodInfo = type.GetMethod(methodName);
            if(methodInfo == null)
            {
                return;
            }
            methodInfo.Invoke(component, null);
        }

        void ApplyProperties(CustomComponentLikeKeySet keySet, Component component)
        {
            var serializedObject = new SerializedObject(component); 

            var jsonStr = (string)this.Config.lookup[keySet.PayloadKey];
            Logger.ImportLog($"custom payload: {jsonStr}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonStr);

            // Logger.ImportLog($"Get the keys: {data.Keys.Count()}");
            foreach (var key in data.Keys)
            {
                var val = data[key];
                // Logger.ImportLog($"KEY: {key} | {val}");
                var property = serializedObject.FindProperty(key);

                Logger.ImportLog($"{key} was a property {property?.propertyPath} isArray : {property?.isArray} | type: {property?.propertyType}");
                if (property == null) { continue; }

                // TODO: (carefully) support instantiating and assigning non-Component but serializable object types
                //  THE THING IS: maybe we can already do this, funnily enough, because wouldn't the sub-fields be addressable
                //    with dot syntax. E.g. if the field is SomeSerOb foo, then key should be "foo.bar" (where bar is property in SomeSerOb)

                // strings qualify as arrays but we don't want to handle them as arrays
                if (property.isArray && property.propertyType != SerializedPropertyType.String) 
                {
                    var vals = ((string)val).Split(','); // our blender data can't do generic arrays right now...
                    serializedObject.FindProperty($"{key}.Array.size").intValue = vals.Length;

                    for (int i = 0; i < vals.Length; ++i)
                    {

                        // Logger.ImportLog($"vals: [{i}]{vals[i]}");
                        var itemAtIProperty = serializedObject.FindProperty($"{key}.Array.data[{i}]");
                        // Logger.ImportLog($"Prop is: {itemAtIProperty.name} prop type: {itemAtIProperty.propertyType}");
                        if (itemAtIProperty.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            this.workTickets.Enqueue(new AssignObjectReferenceWorkTicket
                            {
                                TargetObjectInSceneOrHierarchyName = (string)vals[i],
                                FieldOwner = component,
                                FieldName = key, // $"{key}.Array.data[{i}]" // Dprop.name
                                ArrayIndex = i
                            });
                            continue;
                        }

                        this.AssignFromType(itemAtIProperty, vals[i]);


                    }

                    continue;
                }

                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    this.workTickets.Enqueue(new AssignObjectReferenceWorkTicket
                    {
                        TargetObjectInSceneOrHierarchyName = (string)val,
                        FieldOwner = component,
                        FieldName = property.name
                    });

                    continue;
                }

                this.AssignFromType(property, val);

            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        class AssignObjectReferenceWorkTicket
        {
            public string TargetObjectInSceneOrHierarchyName;
            public Component FieldOwner;
            public string FieldName;
            public int ArrayIndex;
        }

        Queue<AssignObjectReferenceWorkTicket> workTickets = new();

        void AssignObjectReference(AssignObjectReferenceWorkTicket workTicket)
        // void AssignObjectReference(string fieldName, Component component, string targetObjectInScene)
        {
            var targetObjectInScene = workTicket.TargetObjectInSceneOrHierarchyName == ArgonImportSymbols.ThisObject ? 
                                        this.ApplyInfo.Target.name : workTicket.TargetObjectInSceneOrHierarchyName; // targetObjectInScene;

            // try to find the object in this hierarchy
            var tar = ShGameObjectHelper.FindInRoot(this.ApplyInfo.Target.transform, targetObjectInScene);
            if(tar != null) 
            {
                ShGameObjectHelper.AssignObjectReferenceField(workTicket.FieldOwner, workTicket.FieldName, tar.gameObject, workTicket.ArrayIndex);
                return;
            }

            // next, try project assets
            var pob = ShGameObjectHelper.FindInProject<UnityEngine.Object>(targetObjectInScene, "");
            if (pob != null)
            {
                ShGameObjectHelper.AssignObjectReferenceField(workTicket.FieldOwner, workTicket.FieldName, pob, workTicket.ArrayIndex);
                return;
            }

            this.AddAssignFromSceneObject(workTicket.FieldName, workTicket.FieldOwner, targetObjectInScene);
        }

        void AddAssignFromSceneObject(string fieldName, Component component, string targetObjectInScene)
        {
            var assigner = this.ApplyInfo.Target.AddComponent<AssignFieldFromSceneObject>();
            assigner.TargetObjectInScene = targetObjectInScene;
            assigner.FieldOwner = component;
            assigner.FieldName = fieldName;
        }

        bool _parseBool(dynamic val) 
        {
            if (val is bool)
                return val;
            return val > 0 ? true : false;
        }

        void AssignFromType(SerializedProperty property, dynamic val)
        {
            try
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.String:
                        property.stringValue = val;
                        break;
                    case SerializedPropertyType.Integer:
                        property.intValue = val;
                        break;
                    case SerializedPropertyType.Boolean:
                        property.boolValue = this._parseBool(val);
                        break;
                    case SerializedPropertyType.Float:
                        property.floatValue = (float)val; // val might be a double, so cast here
                        break;
                    case SerializedPropertyType.Enum:
                        var type_ = Type.GetType(property.type);
                        if (val is int intval)
                        {
                            property.enumValueIndex = intval;
                            break;
                        }
                        property.enumValueIndex = Array.IndexOf(property.enumNames, val);
                        break;
                    default:
                        Logger.ImportLog($"Prop: {property.propertyType} is not supported.");
                        break;
                }
            }
            catch(RuntimeBinderException rbe)
            {
                throw new RuntimeBinderException($"Key: , path: {property.propertyPath}, property.name: {property.name}, type: {property.propertyType}, val: [{val}] for Target: {this.ApplyInfo.Target.name} \n {rbe.Message}");
            }

        }

        public void Link(ModelPostProcessInfo mppi)
        {
            this.ProcessWorkTickets();
            this.CallLinkStageCallbacks();
        }

        void ProcessWorkTickets()
        {
            while(this.workTickets.Count > 0)
            {
                var ticket = this.workTickets.Dequeue();
                if(string.IsNullOrEmpty(ticket.TargetObjectInSceneOrHierarchyName))
                    continue; 
                var ser = new SerializedObject(ticket.FieldOwner);
                var property = ser.FindProperty(ticket.FieldName);
                Logger.ImportLog($"Linking: prop name: {property.name}");
                this.AssignObjectReference(ticket); 
                ser.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        
        public void CallLinkStageCallbacks()
        {
            
            foreach(var keySet in this.GetKeySets())
            {
                if (string.IsNullOrEmpty(keySet.PayloadKey))
                    continue;

                if (!this.Config.lookup.ContainsKey(keySet.PayloadKey))
                    continue;

                var className = this.Config.lookup[keySet.ApplyClassKey];
                var namespaceString = !string.IsNullOrEmpty(keySet.ApplyClassNamespaceKey) 
                                        && this.Config.lookup.ContainsKey(keySet.ApplyClassNamespaceKey) ?
                                                this.Config.lookup[keySet.ApplyClassNamespaceKey] : "";

                System.Type type = MelGameObjectHelper.FindType(className, namespaceString);

                Assert.IsFalse(type == null, $"No type found for className: '{className}' , namespace '{namespaceString}'");

                var component = this.ApplyInfo.Target.GetComponent(type);

                this.MethodCall(component, type, "_ARGON_OnLink", this.GetImportTimeConfigData(keySet));
                
            }
        }
    }


}
