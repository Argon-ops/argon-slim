using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public abstract class AbstractOvertimeCommand : AbstractCommand
    {
        public bool AllowsInterrupts;
        bool _IsRunning => this._currentTask != null && this._currentTask.Status <= TaskStatus.Running;

        public override void Execute(CommandInfo commandInfo)
        {
            if (!this.AllowsInterrupts && this._IsRunning)
            {
                return;
            }
            base.Execute(commandInfo);
        }


    }
}