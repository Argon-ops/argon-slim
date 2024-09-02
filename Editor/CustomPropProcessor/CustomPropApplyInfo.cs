using UnityEditor;
using UnityEngine;

namespace DuksGames.Tools
{
    public class CustomPropApplyInfo
    {
        public GameObject Target;
        public PostProcessorRouter Postprocessor;
    }

    public static class _CustomPropApplyInfoExtensions
    {
        public static ModelImporter GetModelImporter(this CustomPropApplyInfo cp)
        {
            if (cp.Postprocessor.assetImporter is ModelImporter modelImporter)
            {
                return modelImporter;
            }
            throw new System.Exception($"You are asking for an assetImporter of type ModelImporter but the assetImporter for import '{cp.Target?.name}' is of type {cp.Postprocessor.assetImporter.GetType()} ");
        }
    }
}