using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public class SliderColliderKeySet : AbstractCustomPropKeySet<SliderColliderProcessor>
    {
        public override string TargetKey => "mel_slider_collider";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_axis");
            yield return this.AppendSuffix("_invert");
            yield return this.AppendSuffix("_target_type");
            yield return this.AppendSuffix("_action");
            yield return this.AppendSuffix("_target");
        }
    }

    public class SliderColliderProcessor : AbstractCustomPropProcessor, IIntermediateProductConsumer
    {
        void AddColliderIfNone()
        {
            if(this.ApplyInfo.Target.GetComponent<BoxCollider>() != null) {
                return;
            }
            Logger.ImportLog($"Argon: the object '{this.ApplyInfo.Target.name}' requires a Collider so this script is adding one.");
            var bcoll = this.ApplyInfo.Target.AddComponent<BoxCollider>();
            bcoll.isTrigger = true;
        }

        public void Consume(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            this.AddColliderIfNone();

            var sliderCollider = this.ApplyInfo.Target.AddComponent<SliderCollider>();
            
            sliderCollider.axis = this.GetIntWithSuffix("_axis");
            sliderCollider.invert = this.GetBoolWithSuffix("_invert", true);

            var ttype = this.GetIntWithSuffix("_target_type");
            switch (ttype)
            {
                case 0: // None
                default:
                    break;
                case 1: // Animation

                    var targetName = this.GetStringWithSuffix("_target");
                    // Construct a playable clip wrapper and a PlayableScalarAdapter
                    var target = MelGameObjectHelper.FindInRoot(this.ApplyInfo.Target.transform, this.GetStringWithSuffix("_target"), mppi.ImportHierarchyLookup);

                    var clip = intermediateProductSet.FindClip(target, this.GetStringWithSuffix("_action"), mppi.ImportHierarchyLookup);
                    var pci = new PlayableClipIngredients()
                    {
                        AnimationClips = new AnimationClip[] { clip },
                        // need to tell the pci what the target is, so it can find an animator 
                        AnimationTargets = new GameObject[] { target.gameObject },
                        AudioClip = null,
                        AudioSource = null,
                        ShouldLoopAudio = false,
                        AudioAlwaysForwards = false,
                        GraphName = $"PCI_SC_{target.name}",
                    };
                    var scalAdapter = MelGameObjectHelper.AddIfNotPresent<PlayableScalarAdapter>(this.ApplyInfo.Target);
                    scalAdapter.clipWrapper = CommandFactory.FindClipWrapper(this.ApplyInfo.Target.transform, pci);
                    Assert.IsFalse(scalAdapter.clipWrapper == null, "Null clip wrapper");
                    sliderCollider.ISignalHandlerLinks.Add(scalAdapter);
                    break;
                case 2: // Target object
                    // TODO. imple. simply attach any ISignalHandlers that we find
                    throw new System.Exception("the Target Object option for SliderCollider isn't supported at this time");
            }


        }
    }
}