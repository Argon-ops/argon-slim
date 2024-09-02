using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class TargetObjectProcessorRouter : AssetPostprocessor
    {
        AbstractTargetObjectProcessor[] targetObjectProcessors = new AbstractTargetObjectProcessor[] {
            // ScriptableObject.CreateInstance<SwapMaterialObjectProcessor>(), 
            // No implementers any longer 
        };

        void OnPostprocessGameObjectWithUserProperties(
            GameObject go,
            string[] propNames,
            System.Object[] values) 
        {
            // call target object post processor 
            foreach(var targetObjectProcessor in this.targetObjectProcessors) {
                if(targetObjectProcessor is IPostProcessWithProperties) {
                    ((IPostProcessWithProperties)targetObjectProcessor).PostProcessWithProperties(go, propNames, values);
                }
            }
        }

        void OnPostprocessModel(GameObject root) 
        {
            var modelPostProcessInfo = new ModelPostProcessInfo
            {
                Root = root,
                AssetPostprocessor = this
            };
            foreach(var top in this.targetObjectProcessors) {
                if(top is IModelPostProcessor) {
                    ((IModelPostProcessor)top).PostProcessModel(modelPostProcessInfo);
                }
            }
        }
    }

}