using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Event;
using System.Collections.Generic;
using UnityEngine.Events;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;
using DuksGames.Argon.PropertyDrawers;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{

    public class ClickInteractionHandler : MonoBehaviour,
                                            IClickInteractionHandler,
                                            ISleep
    {
        [TypeEnforce(typeof(IExecute))]
        public Component[] IExecuteLinks;
        protected IEnumerable<IExecute> Commands => this.IExecuteLinks.CastOrGet<IExecute>();

        [TypeEnforce(typeof(ICommandSignalSource))]
        public Component ICommandSignalSourceLink;
        protected ICommandSignalSource SignalSource => this.ICommandSignalSourceLink.CastOrGet<ICommandSignalSource>();

        bool _enabled = true;

        public UnityEvent<InteractionHandlerInfo> OnInteracted = new();


        protected CommandInfo CreateCommandInfo(InteractionHandlerInfo handlerInfo, double signal)
        {
            return CommandInfoFactory.CreateCommandInfo(handlerInfo, signal);
        }

        public bool CouldInteract(InteractionHandlerInfo handlerInfo)
        {
            return this._enabled;
            // TODO: mechanism / convention where we check if we have--say--run out of commands that are enabled...
        }

        public virtual void Interact(InteractionHandlerInfo handlerInfo)
        {
            if (!this._enabled)
                return;

            if (this.IExecuteLinks.Length == 0)
                Debug.LogWarning($"There are no commands on this handler: {this.name} ");


            try
            {
                var commandInfo = this.CreateCommandInfo(handlerInfo, this.SignalSource.GetEnterSignal());
                foreach (var cmd in this.Commands)
                {
                    cmd.Execute(commandInfo);
                }
            }
            catch (System.InvalidCastException ice)
            {
                throw new System.InvalidCastException(
                    $"Invalid Cast: Handler: {this.name} with {this.IExecuteLinks.Length}  Components: {this.IExecuteLinks.JoinSelf(l => l.name + ':' + l.GetType())} || {ice.ToString()}");
            }

            this.OnInteracted.Invoke(handlerInfo); 
        }

        public void SetIsAwake(bool isAwake)
        {
            this._enabled = isAwake;
            this.GetComponent<InteractionSleepSpreader>().HandleIsAwake(isAwake);
        }

    }
}