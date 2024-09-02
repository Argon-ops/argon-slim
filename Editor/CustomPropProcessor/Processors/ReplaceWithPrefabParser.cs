using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

namespace DuksGames.Tools
{
    public class ReplaceWithPrefabKeySet : AbstractCustomPropKeySet<ReplaceWithPrefabParser>
    {
        public override string TargetKey => "mel_replace_with_prefab";

        public override IEnumerable<string> GetKeys()
        {
            // yield return this.TargetKey;
            yield return this.AppendSuffix("_prefab_name");
            // yield return this.AppendSuffix("_parent_adopts_prefab"); // required
            yield return this.AppendSuffix("_prefab_adopts_children");
            yield return this.AppendSuffix("_destroy_target");
            yield return this.AppendSuffix("_match_position");
            yield return this.AppendSuffix("_match_rotation");
            yield return this.AppendSuffix("_compensate_import_rotation");
            yield return this.AppendSuffix("_match_scale");

        }

    }
    public class ReplaceWithPrefabParser : AbstractCustomPropProcessor, IModelPostProcessor, ICleanupProcess
    {

        void _Replace(Transform target)
        {
            var prefab = MelGameObjectHelper.FindInProjectOrWarn<GameObject>(
                                this.GetStringWithSuffix("_prefab_name"), "prefab", $" Search for {this.GetStringWithSuffix("_prefab_name")} failed. ReplaceWithPrefabParser attached to object: {this.ApplyInfo.Target.name}");

            Assert.IsFalse(prefab == null, $"Did not find a prefab named: {this.GetStringWithSuffix("_prefab_name")}");
            var replacer = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            // incorporate the replaced target's name to fend off duplicate names amoung sibling objects 
            replacer.name = $"{target.name}_{prefab.name}";
            target.name = $"{target.name}_prev";

            if (this.GetBoolWithSuffix("_match_rotation", true))
            {
                replacer.transform.rotation = target.transform.rotation;

                if(this.GetBoolWithSuffix("_compensate_import_rotation", true))
                    replacer.transform.rotation = target.transform.rotation * Quaternion.Euler(90, 0, 0);
            }
            if(this.GetBoolWithSuffix("_match_position", true))
                replacer.transform.position = target.transform.position;

            if(this.GetBoolWithSuffix("_match_scale", true))
                replacer.transform.localScale = target.transform.localScale;

            // Required otherwise the import script spams the prefab into the scene; new one each import; the previous prefab isn't found and destroyed.
            replacer.transform.SetParent(target.parent);

            if(this.GetBoolWithSuffix("_prefab_adopts_children", true))
                while(target.childCount > 0)
                {
                    target.transform.GetChild(0).SetParent(replacer.transform);
                }
        }

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            this._Replace(this.ApplyInfo.Target.transform);
        }


        public void Cleanup(ModelPostProcessInfo mppi)
        {
            // Destroy the target after everything else has run
            if(this.GetBoolWithSuffix("_destroy_target", true))
                GameObject.DestroyImmediate(this.ApplyInfo.Target, false);
        }
    }
}