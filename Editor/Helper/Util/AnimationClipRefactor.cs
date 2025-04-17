using UnityEngine;
using UnityEditor;
using System.Linq;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using System.IO;
using DuksGames.Argon.Shared;
using System.Collections;
using System.Collections.Generic;

namespace DuksGames.Tools
{

    public static class AnimationClipRefactor
    {
        // [MenuItem("Tools/Argon/Refactor Animation Clips: Extend to ancestors", false)]
        static void _Refactor()
        {
            var target = Selection.activeGameObject.transform;
            AnimationClipRefactor.RefactorImpl(target);
        }

        static void RefactorImpl(Transform target)
        {
            Logger.Log($"refactoring: {target.name}");

            // AssetDatabase.StartAssetEditing();

            try
            {

                var animator = target.FindInAncestorSelfInclusive<Animator>();
                var clips = animator.runtimeAnimatorController.animationClips;

                Logger.Log($"NUM CLIPS: {clips.Length}");

                foreach (var cl in clips)
                {
                    var bindings = AnimationUtility.GetCurveBindings(cl);
                    var paths = bindings.JoinSelf(bi => $"{bi.propertyName}: {bi.path} \n");
                    Logger.Log($"Clip {cl.name} : {paths}");
                }

                RefactorClipPaths(animator, clips, true);

                
                // animator.cont = clips;
                // AssetDatabase.CreateAsset()

                EditorUtility.SetDirty(animator);

                // NOT NEEDED...
                var ser = new SerializedObject(animator);
                ser.Update();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                // AssetDatabase.StopAssetEditing();
            }

        }

        static AnimationClip MakeACopy(AnimationClip clip)
        {
            var opath = AssetDatabase.GetAssetPath(clip);
            var path = $"{opath}";
            path = path.Substring(0, path.Length - 5) + "_2.anim";
            Logger.Log($"Path: {path} \n opath: {opath}");
            if (AssetDatabase.CopyAsset(opath, path))
            {
                var copy = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                Logger.Log($"DID it. new path: {path}");
                return copy;
            }

            throw new System.Exception("Failed to copy");

        }

        
        static void RefactorClipPaths(Animator animator, AnimationClip[] clips, bool extendPathToAncestors)
        {
            for (int i = 0; i < clips.Length; ++i)
            {
                var oclip = clips[i];
                var clip = MakeACopy(oclip);
                var bindings = AnimationUtility.GetCurveBindings(clip);

                for (int j = 0; j < bindings.Length; ++j) 
                {
                    var binding = bindings[j];
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    var objectRefCurve = AnimationUtility.GetObjectReferenceCurve(clip, binding);

                    binding.path = extendPathToAncestors ?
                            RecalculatePathAncestors(animator.transform, binding.path) :
                            RecalculatePath(animator.transform, binding.path);

                    Logger.Log($"GOT NEXT PATH: {binding.path}".Green());

                    if(curve != null)
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                    else
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, objectRefCurve);
                }

                
                // // AssetDatabase.SaveAssets();
                // EditorUtility.SetDirty(clip);
                // AssetDatabase.SaveAssetIfDirty(clip);

                // var folder = "Assets/DTestEditedClips";
                // var clipName = clip.name.Replace('|', '_');
                // // AssetDatabase.CreateAsset(clip, $"{folder}/{clipName}.anim");
                // Logger.Log(AssetDatabase.GetAssetPath(clip).Pink());

            }
        }

        static string RecalculatePathAncestors(Transform nextRoot, string currentPath)
        {
            var pathComponents = currentPath.Split('/');
            var last = pathComponents.Last();
            var lastTransform = nextRoot.FindRecursive(last);

            // Assert.IsFalse(lastTransform == null);
            if (lastTransform == null)
                return string.Empty;

            var result = lastTransform.PathUpToButNotIncludingParent(nextRoot);
            Logger.Log($"WAS/IS\n{currentPath}\n{result}");
            return result;
        }
        
        static string RecalculatePath(Transform nextRoot, string currentPath)
        {
            var pathComponents = currentPath.Split('/');
            var last = pathComponents.Last();

            // Path should not include the intended root object (so empty string to target this object)
            if (last == nextRoot.name) { return string.Empty; } 

            // Get the descendant transforms up to nextRoot
            var reverseDescendants = pathComponents.Reverse().Select(childName => nextRoot.FindRecursive(childName)); 

            var res = string.Empty;
            foreach(var child in reverseDescendants) 
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

            Logger.Log($"WAS/IS: \n{currentPath}\n{res}");
            return res;
        }

        //dicey what does this do?
        static private Animator animatorObject;
        // static private List<AnimationClip> animationClips;
        static private ArrayList pathsKeys;
        static private Hashtable paths;

        static void FillModel(AnimationClip[] animationClips) {
            paths = new Hashtable();
            pathsKeys = new ArrayList();

            foreach ( AnimationClip animationClip in animationClips )
            {
                FillModelWithCurves(AnimationUtility.GetCurveBindings(animationClip));
                FillModelWithCurves(AnimationUtility.GetObjectReferenceCurveBindings(animationClip));
            }
        }
        
        static void FillModelWithCurves(EditorCurveBinding[] curves) {
            foreach (EditorCurveBinding curveData in curves) {
                string key = curveData.path;
                
                if (paths.ContainsKey(key)) {
                    ((ArrayList)paths[key]).Add(curveData);
                } else {
                    ArrayList newProperties = new ArrayList();
                    newProperties.Add(curveData);
                    paths.Add(key, newProperties);
                    pathsKeys.Add(key);
                }
            }
        }

    }
}