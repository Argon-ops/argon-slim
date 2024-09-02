using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class ObjectEnableKeySet : EnableableKeySet<ObjectEnableProcessor> // AbstractCustomPropKeySet<ObjectEnableProcessor>
    {
        public override string TargetKey => "mel_object_enable";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_invert");
        }
    }

    public class ObjectEnableProcessor : EnableablePropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var objectEnable = this.ApplyInfo.Target.AddComponent<ObjectEnable>();

            this.ApplyToEnableable(objectEnable);
        }
    }
}