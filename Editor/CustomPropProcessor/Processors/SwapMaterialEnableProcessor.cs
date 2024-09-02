using System.Collections.Generic;
using DuksGames.Argon.Core;
using UnityEngine;

namespace DuksGames.Tools
{
    public class SwapMaterialEnableKeySet : EnableableKeySet<SwapMaterialEnableProcessor>
    {
        public override string TargetKey => "mel_swap_material_enable";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_material");
        }
    }

    public class SwapMaterialEnableProcessor : EnableablePropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var swapMatEnable = this.ApplyInfo.Target.AddComponent<SwapMaterialEnable>();

            var materialName = this.GetStringWithSuffix("_material");
            swapMatEnable.HighlightMat = MelGameObjectHelper.FindInProjectOrWarn<Material>(materialName,
                        "Material", $"Failed to find highlight material named: '{materialName}' ");

            this.ApplyToEnableable(swapMatEnable);
        }
    }
}