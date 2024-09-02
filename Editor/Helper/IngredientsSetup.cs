using UnityEngine;
using UnityEditor;
using System.Linq;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using System.IO;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{

    public class IngredientSetupException : System.Exception
    {
        public IngredientSetupException(string info) : base(info) {}
    }

    public static class IngredientsSetup 
    {

        static bool NeedsAnimator(PlayableClipIngredients pci) {
            return pci.AnimationClips != null && pci.AnimationClips.Length > 0;
        }
        
        static Transform ClosestCommonParent(GameObject[] animationTargets) {

            // Each anim-command gets its own animator.
            //   The PlayableGraph just needs any old animator to work.  But there's a catch:
            //     Animators not attached to model root break the animations (nothing animates). 
            //       This is because the animation's paths-to-traverse-hierarchy doesn't update 
            //         when it's attached to a new animator. This is why we are finding a common
            //           parent among animation targets and refactoring animation paths.
                   
            if (animationTargets == null) {
                return null;
            }
            if (animationTargets.Length == 0) {
                return null;
            }
            if (animationTargets.Length == 1) {
                // the easy case
                return animationTargets[0].transform;
            }
            
            return MelGameObjectHelper.CommonParent(animationTargets.Select(g => g.transform).ToArray());
        }

        static string RecalculatePath(Transform nextRoot, string currentPath)
        {
            var pathComponents = currentPath.Split('/');
            var last = pathComponents.Last();

            // Path should not include the intended root object (so empty string to target this object)
            if (last == nextRoot.name) { return string.Empty; } 

            // TODO: test if this works when currentPath's current root is buried somewhere
            //   under nextRoot
            //  Example:
            //  Hierarchy: NextRoot > ChA > ChB > CurrPathRoot > CurrPathChX > CurrPathChY
            //  wouldn't this just produce reverseParents like: CurrPathChY, CurrPathChX, CurrPathRoot
            ///   Which would not, in turn, produce a corrent path (we'd want ChB and ChA to be included, no?)
            ///   
            ///   THE ABOVE IS: not wrong. This method (silently) assumes that nextRoot is a MEMBER of currentPath
            ///     in other words, it is the one that is buried in the hierarchy--and this does make sense
            ///       when its importing an FBX.
            var reverseParents = pathComponents.Reverse().Select(childName => nextRoot.FindRecursive(childName)); 

            var res = string.Empty;
            foreach(var child in reverseParents) 
            {
                if (!child) 
                {
                    break;
                }
                if (child == nextRoot) 
                { 
                    // this should never happen actually (FindRecursive won't find our root Transfrom)
                    break;
                }
                res = string.IsNullOrEmpty(res) ? child.name : $"{child.name}/{res}";
            }
            return res;
        }

        public static void RefactorClipPaths(Animator animator, AnimationClip[] clips)
        {
            IngredientsSetup.RefactorClipPathsImpl(animator, clips);
        }

        static void RefactorClipPathsImpl(Animator animator, AnimationClip[] clips)
        {
            for (int i = 0; i < clips.Length; ++i)
            {
                var clip = clips[i];
                var bindings = AnimationUtility.GetCurveBindings(clip);

                for (int j = 0; j < bindings.Length; ++j) 
                {
                    var binding = bindings[j];
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    var objectRefCurve = AnimationUtility.GetObjectReferenceCurve(clip, binding);

                    binding.path = IngredientsSetup.RecalculatePath(animator.transform, binding.path);

                    if(curve != null)
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                    else
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, objectRefCurve);
                }
            }
        }

        public static RTPlayableClipIngredients Create(GameObject owner, PlayableClipIngredients pci)
        {
            var rt = RTPlayableClipIngredients.From(pci);

            if (NeedsAnimator(pci)) 
            {
                // Attach an animator. Try to find a common parent of the targets.
                //  because each animation command needs its own exclusive animator; no other animation commands should
                //    refer to it. Want this because otherwise one animation doesn't hold position when it's animator
                //      gets asked to do another job: e.g. door animates open. but when another door animates, the first door closes. 
                //         (their shared animator gets 'distracted')
                //   COMPLAINT: the user gets this benefit only sometimes (i.e. when we succeed in finding the common parent) 
                //      and the reasons for the sometimes will be very opaque!   
                var commonParent = IngredientsSetup.ClosestCommonParent(pci.AnimationTargets);
                rt.Animator = commonParent == null ? 
                    MelGameObjectHelper.AddIfNotPresent<Animator>(owner.transform.root.gameObject) : 
                    MelGameObjectHelper.AddIfNotPresent<Animator>(commonParent.gameObject); 

                // Refactor in the case where the animator is not attached to the root object
                //  TODO: determine if we need to copy these clips. Editing clip paths is only ok if
                //    the clips will only ever have one user. Which seems like a pretty bad assumption!
                //     Then again, making lots of unneeded copies of animations doesn't seem great either.
                //  TODO: at least give the user the option to opt out of this ClosestParent animator + refactor business.
                //     And try our best to explain why in the world they'd want or not want it..
                IngredientsSetup.RefactorClipPathsImpl(rt.Animator, pci.AnimationClips);
                rt.AnimationClips = pci.AnimationClips;
            }
            return rt;
        }
    }
}