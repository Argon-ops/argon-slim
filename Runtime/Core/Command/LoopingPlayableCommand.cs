using System.Threading.Tasks;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Event;
using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class LoopingPlayableCommand : PlayPlayableCommand
    {
        protected override void HandlePlayEnded(LimitNotifyPlayable.PlayCursorData data)
        {
            this.Playable.RestartPlay(true); // loops only go forwards
        }

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            if (commandInfo.Signal > .5f)
            {
                if (this.IsNotPlaying)
                {
                    return base._Execute(commandInfo);
                }

                // already playing. just exit
                return Task.FromResult(CommandResult.NeverStarted(commandInfo));
            }

            this.InterruptPreviousPlay();

            this.Playable.Stop();

            // return the task right away. nothing asynchronous to wait for when interrupting
            return Task.FromResult(CommandResult.NeverStarted(commandInfo));
        }

        protected override void RestartPlay(CommandInfo commandInfo)
        {
            this.Playable.Play();
        }
    }
}