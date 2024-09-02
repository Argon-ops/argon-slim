using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public class MessageBusEventCommand : AbstractCommand
    {
        public GameObject[] Targets;
        public string MessageBusType;

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            MessageBus.Instance.Broadcast(new MessageBusEvent
            {
                Type = this.MessageBusType,
                Targets = this.Targets,
                CommandInfo = commandInfo,
                CustomInfo = this.CustomInfo,
            });

            return Task.FromResult(new CommandResult
            {
                Type = CommandResultType.Completed,
                CommandInfo = commandInfo
            });

        }

    }
}
