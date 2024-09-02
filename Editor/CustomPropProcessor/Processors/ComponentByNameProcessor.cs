using UnityEngine;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class ComponentByNameKeySet : AbstractCustomPropKeySet<ComponentByNameProcessor>
    {
        public override string TargetKey => "mel_component_by_name";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_type_name");
            yield return this.AppendSuffix("_namespace");
        }
    }

    public class ComponentByNameProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var type = MelGameObjectHelper.FindType(
                this.GetStringWithSuffix("_type_name"),
                this.GetStringWithSuffix("_namespace"));

            if (type == null)
            {
                Debug.LogWarning($"ComponentByNameProcessor: type: [{this.GetStringWithSuffix("_type_name")}] with namespace[{this.GetStringWithSuffix("_namespace")}] not found. ");
                return;
            }
            ApplyInfo.Target.AddComponent(type);

        }
    }
}