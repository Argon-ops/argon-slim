using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using UnityEngine.VFX;

namespace DuksGames.Tools
{

    public class VisualEffectKeySet : EnableableKeySet<VisualEffectProcessor>
    {
        public override string TargetKey => "mel_visual_effect";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.TargetKey;
            yield return this.AppendSuffix("_initial_state");
            yield return this.AppendSuffix("_toggle_game_object");
        }
    }

    public class VisualEffectProcessor : EnableablePropProcessor, IModelPostProcessor
    {

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            var vfxPrefab = MelGameObjectHelper.FindInProjectOrWarn<GameObject>(this.GetStringWithSuffix(""), "prefab");

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(vfxPrefab, this.ApplyInfo.Target.transform);
            var vfx = instance.GetComponentInChildren<VisualEffect>();

            vfx.transform.rotation *= Quaternion.Inverse(this.ApplyInfo.Target.transform.rotation);

            var vfxEnable = this.ApplyInfo.Target.gameObject.AddComponent<VisualEffectEnable>();
            vfxEnable.EffectLink_ = vfx;
            vfxEnable.ToggleGameObject = this.GetBoolWithSuffix("_toggle_game_object", true);

            this.ApplyToEnableable(vfxEnable);
        }
    }
}