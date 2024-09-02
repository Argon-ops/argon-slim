using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Animate;
using System.Linq;

namespace DuksGames.Tools
{
    public class PCWrapperGenerator : AbstractGlobalsImportSettingsConsumer
    {
        public override void ConsumeGlobals(GameObject root, IntermediateProductSet intermediateProductSet)
        {
            return;
            // turn off globals for now. we're using the force-per object strategy
            // which clips do we want
            // var clipNames = intermediateProductSet.Clips.Keys.Where(clipName =>
            // {
            //     if (intermediateProductSet.GlobalImportSettings.pcwForAllClips) return true;
            //     return false;
            // });

            // foreach(var clipName in clipNames) 
            // {
            //     var clip = intermediateProductSet.Clips[clipName];

            //     // TODO: it may be good to find the target object specified in the first part of clip name:
            //     //    e.g. "TargetName|ClipName"

            //     var pci = new PlayableClipIngredients()
            //     {
            //         AnimationClips = new AnimationClip[] { clip },
            //         AnimationTargets = new GameObject[] { root },
            //         AudioClip = null, 
            //         AudioSource = null, 
            //         ShouldLoopAudio = false, 
            //         AudioAlwaysForwards = true, 
            //         GraphName = $"ArgonPCW_{clipName}",
            //     };
            //     CommandFactory.FindClipWrapper(root.transform, pci);
            // }
        }
    }
}