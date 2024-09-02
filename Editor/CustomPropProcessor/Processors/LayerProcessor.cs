using System.Collections.Generic;
using UnityEngine;

namespace DuksGames.Tools
{
    public class LayerInterpreterKeySet : AbstractCustomPropKeySet<LayerProcessor>
    {
        public override string TargetKey => "mel_layer";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.TargetKey;
        }

    }

    public class LayerProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var layerName = Config.getString(KeySet.TargetKey);
            var matchingName = MelGameObjectHelper.AddLayer(layerName.Trim());
            ApplyInfo.Target.layer = LayerMask.NameToLayer(matchingName);
        }
    }
}