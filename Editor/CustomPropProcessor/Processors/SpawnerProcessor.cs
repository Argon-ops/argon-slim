using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class SpawnerKeySet : EnableableKeySet<SpawnerProcessor> //  AbstractCustomPropKeySet<SpawnerProcessor>
    {
        public override string TargetKey => "mel_spawner";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_prefab_name");
            yield return this.AppendSuffix("_spawn_mode");
            yield return this.AppendSuffix("_spawn_interval");
        }
    }

    public class SpawnerProcessor : EnableablePropProcessor, IApplyCustomProperties
    {

        ContinuousSpawner AddContinuous(GameObject prefab)
        {
            var continuous = this.ApplyInfo.Target.AddComponent<ContinuousSpawner>();
            continuous.Prefab = prefab;
            continuous.Interval = this.GetFloatWithSuffix("_spawn_interval");
            return continuous;
        }

        Spawner AddSpawner(GameObject prefab)
        {
            var spawner = this.ApplyInfo.Target.AddComponent<Spawner>();
            spawner.Prefab = prefab;
            return spawner;
        }


        public void Apply()
        {
            var name = this.GetStringWithSuffix("_prefab_name");
            var prefab = MelGameObjectHelper.FindInProjectOrWarn<GameObject>(name, "prefab", $"Request from SpawnerProcessor attached to: '{this.ApplyInfo.Target.name}' ");

            this.ApplyToEnableable(this.GetIntWithSuffix("_spawn_mode") == 1 ? 
                                                            this.AddContinuous(prefab) : 
                                                            this.AddSpawner(prefab));
        }
    }
}