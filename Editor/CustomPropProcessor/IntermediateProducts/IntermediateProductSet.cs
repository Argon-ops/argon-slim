using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DuksGames.Tools
{

    public interface IIntermediateProductProducer
    {
        void SetProductSet(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet);
    }

    public interface IIntermediateProductConsumer
    {
        void Consume(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet);
    }

    // For processes that need components from other processes to already exist,
    //  Link gets called after all the other set up funcs except Cleanup
    public interface ILinkerProcess
    {
        void Link(ModelPostProcessInfo mppi);
    }

    public interface ICleanupProcess
    {
        void Cleanup(ModelPostProcessInfo mppi);
    }

    public class IntermediateProductSet
    {
        public EditorPlayableClipShare PlayableFabInfoShare = new();

        public Dictionary<string, AnimationClip> Clips = new();

        public GlobalImportSettings GlobalImportSettings = new();

        public AnimationClip FindClip(Transform target, string anim_name, ImportHierarchyLookup importHierarchyLookup)
        {
            try
            {
                return FindClip(target.name, anim_name);
            }
            catch (System.InvalidOperationException)
            {
                // because sometimes the unity imported name does not equal the import name
                if (importHierarchyLookup.TryGetImportName(target.gameObject.GetInstanceID(), out string importName))
                {
                    return FindClip(importName, anim_name);
                }
                throw new System.Exception($"This won't ever happen");
            }

        }

        public AnimationClip FindClip(string clipName)
        {
            return this.Clips.Values.First(clip => clip.name == clipName);
        }

        public AnimationClip FindClip(string targetName, string anim_name)
        {

            try
            {
                return this.Clips.Values.First(clip => clip.name == $"{targetName}|{anim_name}");
            }
            catch (System.ArgumentNullException ne)
            {
                Debug.LogWarning($"No animation clip for target '{targetName}' and blender action '{anim_name}'. Looking for a clip named: '{targetName}|{anim_name}'. You may have chosen a target that wasn't associated with the action. Perhaps (for example, just a guess) an object that is controlled by an armature instead of the armature itself? ");
                throw ne;
            }
            catch (System.InvalidOperationException ioe)
            {
                throw new System.InvalidOperationException($"No animation clip for target '{targetName}' and blender action '{anim_name}'. Looking for a clip named: '{targetName}|{anim_name}'. You may have chosen a target that wasn't associated with the action. Perhaps (for example, just a guess) an object that is controlled by an armature instead of the armature itself? {ioe}");
            }
        }

        public void AddClip(AnimationClip clip)
        {
            this.Clips.Add(clip.name, clip);
        }

        public void Clear()
        {
            this.PlayableFabInfoShare.Clear();
            this.Clips.Clear();
        }
    }



}