using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;

namespace DuksGames.Argon.Core
{
    public class HeadlineCommand : AbstractCommand
    {
        public string Text;

        public float Seconds;

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            if (commandInfo.Signal < .5f)
            {
                return this.TurnOff(commandInfo);
            }

            return this.TurnOn(commandInfo);
        }

        Task<CommandResult> TurnOn(CommandInfo commandInfo)
        {
            var tcs = new TaskCompletionSource<CommandResult>();
            this.AwaitHeadline(commandInfo, tcs);
            return tcs.Task;
        }

        Task<CommandResult> TurnOff(CommandInfo commandInfo)
        {
            return Task.FromResult(CommandResult.CompletedResult(commandInfo));
        }

        async void AwaitHeadline(CommandInfo commandInfo, TaskCompletionSource<CommandResult> tcs)
        {
            var headline = SceneServices.Instance.HeadlineDisplay;
            await headline.Display(new HeadlineDisplayInfo
            {
                Text = this.Text,
                Seconds = this.Seconds
            });
            tcs.SetResult(CommandResult.CompletedResult(commandInfo));
        }
    }
}