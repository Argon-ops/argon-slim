using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public class ScreenOverlayEnableKeySet : EnableableKeySet<ScreenOverlayEnableProcessor> //  AbstractCustomPropKeySet<ScreenOverlayEnableProcessor>
    {
        public override string TargetKey => "mel_screen_overlay_enable";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_overlay_tag");
        }
    }
    public class ScreenOverlayEnableProcessor : EnableablePropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var receiver = this.ApplyInfo.Target.AddComponent<ScreenOverlayEnable>();
            receiver.overlayName = this.GetStringWithSuffix("_overlay_tag");

            this.ApplyToEnableable(receiver);
        }
    }
}