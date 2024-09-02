using System;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using DuksGames.Argon.Shared;
using UnityEngine;

namespace DuksGames.Tools
{

    public abstract class EnableableKeySet<K> : AbstractCustomPropKeySet<K> where K : EnableablePropProcessor, new()
    {

        IEnumerable<string> GetEnableableKeys()
        {
            yield return this.AppendSuffix("_invert");
            yield return this.AppendSuffix("_clamp01");
            yield return this.AppendSuffix("_threshold");
            yield return this.AppendSuffix("_set_initial_state");
            yield return this.AppendSuffix("_is_self_toggling");
        }
        public override sealed IEnumerable<string> GetKeys()
        {
            foreach (var ekey in this.GetEnableableKeys())
            {
                yield return ekey;
            }
            foreach (var key in this.GetAdditionalKeys())
            {
                yield return key;
            }
        }

        public abstract IEnumerable<string> GetAdditionalKeys();
    }

    public class EnableablePropProcessor : AbstractCustomPropProcessor
    {

        protected void ApplyToEnableable(AbstractThresholdInterpreter enableable)
        {
            this.ApplyThresholdSettings(enableable);
            this.ApplyInitialState(enableable);
        }

        void ApplyThresholdSettings(AbstractThresholdInterpreter enableable)
        {
            enableable.thresholdSettings.Invert = this.GetBoolWithSuffix("_invert", true);
            enableable.thresholdSettings.Clamp01 = this.GetBoolWithSuffix("_clamp01", true);
            enableable.thresholdSettings.Threshold = this.GetFloatWithSuffix("_threshold");
            enableable.IsSelfToggling = this.GetBoolWithSuffix("_is_self_toggling", true);
        }

        private void ApplyInitialState(AbstractThresholdInterpreter enableable)
        {
            if (this.GetIntWithSuffix("_set_initial_state") == (int)TurnOnOffDirectiveType.DoNothing)
            {
                return;
            }

            var setter = enableable.gameObject.AddComponent<InitialEnableStateSetter>();
            setter.ISignalHandlerLink = enableable;
            setter.InitialState = this.GetIntWithSuffix("_set_initial_state") == (int)TurnOnOffDirectiveType.TurnOn;
        }
    }
}