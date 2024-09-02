using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using System.Linq;

namespace DuksGames.Tools
{
    public class RETwoPickupSessionKeySet : AbstractCustomPropKeySet<RETwoPickupSessionProcessor>
    {
        public override string TargetKey => "mel_re2_pick_session";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_mode");
            yield return this.AppendSuffix("_should_call_click_handlers");
            yield return this.AppendSuffix("_try_inventory");
            yield return this.AppendSuffix("_cancel_button_0");
            yield return this.AppendSuffix("_cancel_button_1");

        }
    }
    public class RETwoPickupSessionProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var reTwoPickSession = this.ApplyInfo.Target.AddComponent<RETwoPickupSession>();

            reTwoPickSession.ShouldCallClickHandlers = this.GetBoolWithSuffix("_should_call_click_handlers", true);
            reTwoPickSession.TryAddingToInventory = this.GetBoolWithSuffix("_try_inventory", true);
            reTwoPickSession.CancelButtonsNames = (new string[] {
                    this.GetStringWithSuffix("_cancel_button_0"),
                    this.GetStringWithSuffix("_cancel_button_1")
                }).Where(s => !string.IsNullOrEmpty(s)).ToArray();

            var Dkey = this.AppendSuffix("_should_call_click_handlers");
            var Dval = this.GetBoolWithSuffix("_should_call_click_handlers");
        }
    }
}