using UnityEngine;
using DuksGames.Argon.Interaction;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Event;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{

    public class TriggerInteractionHandler : MonoBehaviour,
                                                ITriggerEnterHandler,
                                                ITriggerExitHandler,
                                                ISleep
    {
        public enum EnterExitHandling
        {
            EnterOnly, ExitOnly, EnterAndExit
        }
        public EnterExitHandling EnterExit;

        public Component ICommandSignalSourceLink;
        ICommandSignalSource CommandSignalSource => (ICommandSignalSource)this.ICommandSignalSourceLink;

        public Component[] IExecuteLinks;
        IEnumerable<IExecute> _commands => this.IExecuteLinks.Select(c => (IExecute)c);
        bool _enabled = true;

        public void HandleTriggerEnter(InteractionHandlerInfo handlerInfo)
        {
            Debug.Log($"H trigger enter");
            if (this.EnterExit == EnterExitHandling.ExitOnly)
            {
                return;
            }
            Debug.Log("call Sending command");

            this.SendCommand(handlerInfo, this.CommandSignalSource.GetEnterSignal());
        }

        public void HandleTriggerExit(InteractionHandlerInfo handlerInfo)
        {
            if (this.EnterExit == EnterExitHandling.EnterOnly)
            {
                return;
            }

            this.SendCommand(handlerInfo, this.CommandSignalSource.GetExitSignal());
        }

        void SendCommand(InteractionHandlerInfo handlerInfo, double signal)
        {
            if (!this._enabled)
            {
                return;
            }
            Debug.Log("Sending command");
            var commandInfo_ = CommandInfoFactory.CreateCommandInfo(handlerInfo, signal);

            foreach (var cmd in this._commands)
            {
                cmd.Execute(commandInfo_);
            }
        }

        public void SetIsAwake(bool isAwake)
        {
            this._enabled = isAwake;
            this.GetComponent<InteractionSleepSpreader>().HandleIsAwake(isAwake);
        }
    }
}