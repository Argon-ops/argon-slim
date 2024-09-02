using UnityEngine;
using UnityEditor;

namespace DuksGames.Tools
{
    public class ModelPostProcessInfo
    {
        public GameObject Root;
        public AssetPostprocessor AssetPostprocessor;
        public ImportHierarchyLookup ImportHierarchyLookup;
        public AssociateLookup AssociateLookup;
        
    }

    // for classes that want a callback from Unity's OnPostprocessModel(GameObject root)
    public interface IModelPostProcessor
    {
        void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo);
    }

    public interface IApplyCustomProperties
    {
        void Apply();
    }

    public interface IAnimationClipPostProcessor
    {
        void PostProcessAnimation(GameObject go, AnimationClip clip);
    }

}