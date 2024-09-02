using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using System.Linq;
using System;

namespace DuksGames.Argon.Core
{
    public class WaitSecondsCommand : AbstractCommand
    {
        public float WaitTimeSeconds;

        protected override async Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            await Task.Delay(Mathf.CeilToInt(this.WaitTimeSeconds * 1000F));
            return CommandResult.CompletedResult(commandInfo);
        }

    }
}