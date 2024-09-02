using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{

    public class ParticleSystemKeySet : EnableableKeySet<ParticleSystemProcessor> // AbstractCustomPropKeySet<ParticleSystemProcessor>
    {
        public override string TargetKey => "mel_particle_system";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.TargetKey;
            yield return this.AppendSuffix("_initial_state");
            yield return this.AppendSuffix("_toggle_game_object");
        }
    }

    public class ParticleSystemProcessor : EnableablePropProcessor, IApplyCustomProperties, IModelPostProcessor
    {
        public void Apply()
        {
        }

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            var psPrefab = MelGameObjectHelper.FindInProject<ParticleSystem>(this.GetStringWithSuffix(""), "prefab");
            var particleSystem = (ParticleSystem)PrefabUtility.InstantiatePrefab(psPrefab, this.ApplyInfo.Target.transform);

            particleSystem.transform.rotation *= Quaternion.Inverse(this.ApplyInfo.Target.transform.rotation);

            var psEnable = this.ApplyInfo.Target.gameObject.AddComponent<ParticleSystemEnable>();

            psEnable.ParticleSystem = particleSystem;
            psEnable.ToggleGameObject = this.GetBoolWithSuffix("_toggle_game_object", true);
            particleSystem.gameObject.AddComponent<ParticleStopAction>();

            this.ApplyToEnableable(psEnable);
        }
    }
}