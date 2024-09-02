using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class CamSwapManagedKeySet : AbstractCustomPropKeySet<CamSwapManagedParser>
    {
        public override string TargetKey => "mel_cam_swap_managed";

        public override IEnumerable<string> GetKeys()
        {
            yield return null;
        }
    }

    public class CamSwapManagedParser : AbstractCustomPropProcessor, IModelPostProcessor
    {
        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            this.ApplyInfo.Target.AddComponent<RegisterCamSwapManaged>();
        }
    }
}