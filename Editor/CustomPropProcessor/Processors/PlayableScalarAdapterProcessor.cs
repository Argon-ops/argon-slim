using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Shared;
using UnityEngine.Assertions;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class PlayableScalarAdapterKeySet : AbstractCustomPropKeySet<PlayableScalarAdapterProcessor>
    {
        public override string TargetKey => "mel_playable_scalar_adapter";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_action");
            yield return this.AppendSuffix("_is_clip_name_specified_manually");
            yield return this.AppendSuffix("_clip_name");
            yield return this.AppendSuffix("_target");
            yield return this.AppendSuffix("_audio"); // TODO
        }
    }

    public class PlayableScalarAdapterProcessor : AbstractCustomPropProcessor, IApplyCustomProperties, IIntermediateProductConsumer
    {
        public void Apply()
        {
            this.ApplyInfo.Target.AddComponent<PlayableScalarAdapter>();
        }

        AnimationClip GetClip(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet, Transform target)
        {
            if (!this.GetBoolWithSuffix("_is_clip_name_specified_manually", true))
            {
                try 
                {
                    return intermediateProductSet.FindClip(target, this.GetStringWithSuffix("_action"), mppi.ImportHierarchyLookup);
                }
                catch(System.Exception e)
                {
                    throw new System.Exception($"Failed to find AnimationClip. File: {mppi.Root.name} {e}");
                }
            }

            return intermediateProductSet.FindClip(this.GetStringWithSuffix("_clip_name"));

        }

        public void Consume(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            var targetName = this.GetStringWithSuffix("_target");
            // Construct a playable clip wrapper and a PlayableScalarAdapter
            var target = MelGameObjectHelper.FindInRootOrThrow(this.ApplyInfo.Target.transform, targetName, mppi.ImportHierarchyLookup);

            var clip = this.GetClip(mppi, intermediateProductSet, target); // intermediateProductSet.FindClip(target, this.GetStringWithSuffix("_action"), mppi.ImportHierarchyLookup);

            var pci = new PlayableClipIngredients()
            {
                AnimationClips = new AnimationClip[] { clip },
                AnimationTargets = new GameObject[] { target.gameObject },
                AudioClip = null,
                AudioSource = null,
                ShouldLoopAudio = false,
                AudioAlwaysForwards = false,
                GraphName = $"PCI_SC_{target.name}",
            };

            var scalAdapter = this.ApplyInfo.Target.GetOrAddComponent<PlayableScalarAdapter>();
            scalAdapter.clipWrapper = CommandFactory.FindClipWrapper(this.ApplyInfo.Target.transform, pci);
            Assert.IsFalse(scalAdapter.clipWrapper == null, "Null clip wrapper");
        }
    }
}