using UnityEngine;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{

    public class SignalLowThenHighIndefiniteCommand : SignalLowThenHighCommand
    {

        protected override async Task<CommandResult> _Execute(CommandInfo commandInfo)
        {

            foreach (var messageReceiver in this.MessageReceivers)
            {
                messageReceiver.HandleISignal(commandInfo.Signal > .5f ? this._lowValue : this._highValue);
            }

            await Task.Run(() => { });

            return CommandResult.CompletedResult(commandInfo);
        }
    }
}