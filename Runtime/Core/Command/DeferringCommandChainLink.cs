using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Collections;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Core
{
    public class DeferringCommandChainLink : CommandChainLink
    {
        int invocations;

        protected override IEnumerator WaitForDelay(CommandEvent ce)
        {
            this.invocations++;
            yield return new WaitForSeconds(this.Delay);
            this.invocations--;

            if (invocations == 0)
            {
                foreach (var cmd in this.NextCommands)
                {
                    cmd.Execute(ce.Result.CommandInfo);
                }
            }
        }
    }
}