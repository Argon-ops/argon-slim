using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using System.Linq;
using System;

namespace DuksGames.Argon.Core
{
    public class CompositeCommand : AbstractCommand
    {
        public AbstractCommand[] Commands;

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            var subTasks = this.Commands.Select(c => c.ExecuteAsync(commandInfo)).ToArray();
            var alltasks = Task.WhenAll(subTasks);
            return this.Complete(alltasks);
        }

        private async Task<CommandResult> Complete(Task<CommandResult[]> alltasks)
        {
            return (await alltasks).Last();
        }
    }
}