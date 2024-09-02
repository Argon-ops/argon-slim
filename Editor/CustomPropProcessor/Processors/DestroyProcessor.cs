using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public class DestroyKeySet : AbstractCustomPropKeySet<DestroyProcessor>
    {
        public override string TargetKey => "mel_destroy";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_preserve_children");
        }
    }

    public class DestroyProcessor : AbstractCustomPropProcessor, IApplyCustomProperties, ICleanupProcess
    {
        class DestroyInfo
        {
            public GameObject Target;
            public bool PreserveChildren;
        }

        List<DestroyInfo> Targets = new();

        public void Apply()
        {
            this.Targets.Add(new DestroyInfo
            {
                Target = this.ApplyInfo.Target,
                PreserveChildren = this.GetBoolWithSuffix("_preserve_children", true)
            });
        }

        public void Cleanup(ModelPostProcessInfo mppi)
        {
            foreach (var destroyInfo in this.Targets)
            {
                if (!destroyInfo.Target)
                {
                    continue;
                }
                if (destroyInfo.PreserveChildren)
                {
                    var grandParent = destroyInfo.Target.transform.parent;
                    for (int i = 0; i < destroyInfo.Target.transform.childCount; ++i)
                    {
                        destroyInfo.Target.transform.GetChild(i).SetParent(grandParent);
                    }
                }
                GameObject.DestroyImmediate(destroyInfo.Target);
            }
        }
    }
}