using UnityEngine;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class NoRendererKeySet : AbstractCustomPropKeySet<NoRendererParser>
    {
        public override string TargetKey => "mel_no_renderer";

        public override IEnumerable<string> GetKeys()
        {
            yield return null;
        }
    }

    public class NoRendererParser : AbstractCustomPropProcessor, IModelPostProcessor
    {
        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            GameObject.DestroyImmediate(this.ApplyInfo.Target.GetComponent<Renderer>());
            GameObject.DestroyImmediate(this.ApplyInfo.Target.GetComponent<MeshFilter>());
        }
    }
}