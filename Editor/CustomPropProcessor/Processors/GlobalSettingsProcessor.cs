using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class GlobalSettingsKeySet : AbstractCustomPropKeySet<GlobalSettingsProcessor>
    {
        public override string TargetKey => "mel_global_settings_marker";

        public override IEnumerable<string> GetKeys()
        {
            yield return "mel_global_settings_payload";
        }
    }

    public class GlobalSettingsProcessor : AbstractCustomPropProcessor,
        IIntermediateProductProducer
    {
        public void SetProductSet(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            var json = this.Config.getTypedValue<string>("mel_global_settings_payload");
            JsonUtility.FromJsonOverwrite(json, intermediateProductSet.GlobalImportSettings);
        }

    }
}