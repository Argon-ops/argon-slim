using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class CameraShakeCommand : AbstractCommand
    {

        public CameraShakeConfig ShakeConfig;
        ICameraShake _Shake;
        ICameraShake Shake
        {
            get
            {
                if (this._Shake == null)
                {
                    this._Shake = Camera.main.GetComponentInChildren<ICameraShake>();
                }
                return this._Shake;
            }
        }

        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            if (commandInfo.Signal < .5f)
            {
                this.Shake.StopShaking();
                return Task.FromResult(CommandResult.NeverStarted(commandInfo));
            }

            var tcs = new TaskCompletionSource<CommandResult>();
            this.AwaitShake(commandInfo, tcs);
            return tcs.Task;
        }

        async void AwaitShake(CommandInfo commandInfo, TaskCompletionSource<CommandResult> tcs)
        {
            await this.Shake.Shake(this.ShakeConfig);
            tcs.TrySetResult(CommandResult.CompletedResult(commandInfo));

        }
    }
}