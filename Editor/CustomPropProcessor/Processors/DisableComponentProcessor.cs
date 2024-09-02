using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Core;
using DuksGames.Argon.Shared;
using UnityEngine;

namespace DuksGames.Tools
{
    public class DisableComponentKeySet : AbstractCustomPropKeySet<DisableComponentProcessor>
    {
        public override string TargetKey => "mel_disable_component";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_component_names");
        }
    }

    public class DisableComponentProcessor : AbstractCustomPropProcessor, ICleanupProcess
    {
        public void Cleanup(ModelPostProcessInfo mppi)
        {
            var componentNames = this.GetStringWithSuffix("_component_names").Split(",");
            foreach (var compo in this.ApplyInfo.Target.GetComponents<Component>())
            {
                if (!(compo is Behaviour))
                {
                    continue;
                }

                var behaviour = (Behaviour)compo;
                if (componentNames.Any(n => n.Trim().ToLower() == behaviour.GetType().Name.ToLower()))
                {
                    behaviour.enabled = false;
                }
            }
        }
    }
}