using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Event;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public class FakeCommand : AbstractCommand
    {

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            return Task.FromResult(CommandResult.CompletedResult(commandInfo));
        }
    }
}