using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class ComponentEnableKeySet : AbstractCustomPropKeySet<ComponentEnableProcessor>
    {
        public override string TargetKey => "mel_component_enable";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_type_name");
            yield return this.AppendSuffix("_namespace");
            yield return this.AppendSuffix("_apply_to_self");
            yield return this.AppendSuffix("_search_children");
        }
    }

    public class ComponentEnableProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var componentEnable = this.ApplyInfo.Target.AddComponent<ComponentEnable>();

            componentEnable.ApplyToSelf = this.GetBoolWithSuffix("_apply_to_self", true);
            componentEnable.SearchChildren = this.GetBoolWithSuffix("_search_children", true);
            componentEnable.TypeName = this.GetStringWithSuffix("_type_name");
            componentEnable.Namespace = this.GetStringWithSuffix("_namespace");
        }
    }
}