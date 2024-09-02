using UnityEngine;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{

    public class SignalLowThenHighCommand : SignalMessageCommand
    {

        public OverTimeFunctionParameters Parameters;

        protected float _lowValue => this.Parameters.LeftValue;
        protected float _highValue => this.Parameters.RightValue;
        float IntervalSeconds => this.Parameters.TotalRangeSeconds;

        protected override async Task<CommandResult> _Execute(CommandInfo commandInfo)
        {

            foreach (var messageReceiver in this.MessageReceivers)
            {
                messageReceiver.HandleISignal(this._lowValue);
            }

            await Task.Delay((int)(this.IntervalSeconds * 1000));

            foreach (var messageReceiver in this.MessageReceivers)
            {
                messageReceiver.HandleISignal(this._highValue);
            }

            return CommandResult.CompletedResult(commandInfo);
        }
    }
}