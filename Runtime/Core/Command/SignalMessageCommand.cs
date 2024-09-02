using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Core
{

    public class SignalMessageCommand : AbstractCommand
    {

        public Component[] IEnableReceiverLinks;

        protected IEnumerable<ISignalHandler> MessageReceivers => this.IEnableReceiverLinks.Select(receiver => (ISignalHandler)receiver);

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {

            foreach (var messageReceiver in this.MessageReceivers)
            {
                messageReceiver.HandleISignal(commandInfo.Signal);
            }


            return Task.FromResult(CommandResult.CompletedResult(commandInfo));
        }

    }
}