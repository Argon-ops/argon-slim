using System.Threading.Tasks;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{
    public class CompositeSequentialCommand : CompositeCommand
    {

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            return this.Complete(commandInfo);
        }

        private async Task<CommandResult> Complete(CommandInfo commandInfo)
        {
            for (int i = 0; i < this.Commands.Length; ++i)
            {
                var result = await this.Commands[i].ExecuteAsync(commandInfo);
                await Task.Yield(); // wait a frame: if we don't, this will return after first task
                if (i == this.Commands.Length - 1)
                {
                    return result;
                }
            }
            throw new System.Exception("CompositeSequentialCommand needs at least one Command");
        }
    }
}