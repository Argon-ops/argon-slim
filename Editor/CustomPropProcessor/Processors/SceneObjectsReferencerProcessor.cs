using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class SceneObjectsReferencerKeySet : AbstractCustomPropKeySet<SceneObjectsReferencerProcessor>
    {
        public override string TargetKey => "mel_scene_objects_referencer";
        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_object_name_1");
        }
    }

    public class SceneObjectsReferencerProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var referencer = this.ApplyInfo.Target.AddComponent<SceneObjectsReferencer>();
            referencer.Targets = new string[] { 
                this.GetStringWithSuffix("_object_name_1") 
            };
            Debug.Log($"Scene Ob Referencer with targets: {referencer.Targets[0]}");
        }
    }
}