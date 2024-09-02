using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Shared;
using System.Linq;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class ForcePCWKeySet : AbstractCustomPropKeySet<ForcePCWProcessor>
    {
        public override string TargetKey => "mel_force_pcw";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_any_matching_action");
            yield return this.AppendSuffix("_convenience_link");
        }
    }

    public class ForcePCWProcessor : AbstractCustomPropProcessor, IIntermediateProductConsumer
    {
        public void Consume(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            var clipNames = intermediateProductSet.Clips.Keys.Where(clipName =>
            {
                var parts = clipName.Split('|');
                return parts.Length > 0 && parts[0] == this.ApplyInfo.Target.name;
            });

            foreach (var clipName in clipNames)
            {
                var clip = intermediateProductSet.Clips[clipName];
                var pci = new PlayableClipIngredients()
                {
                    AnimationClips = new AnimationClip[] { clip },
                    AnimationTargets = new GameObject[] { this.ApplyInfo.Target },
                    AudioClip = null,
                    AudioSource = null,
                    ShouldLoopAudio = false,
                    AudioAlwaysForwards = true,
                    GraphName = $"Argon_PerOb_PCW_{clipName}",
                };

                var pcw = CommandFactory.FindClipWrapper(this.ApplyInfo.Target.transform, pci);

            }

        }
    }
}