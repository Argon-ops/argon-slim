using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuksGames.LogicBlocks;
using UnityEngine.Events;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Event;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;
using DuksGames.Argon.Core;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Core
{
    
    public interface ICommandEventProvider
    {
        UnityEvent<CommandEvent> GetCommandEvent();
    }

    public struct CommandEvent
    {
        public CommandEventType Type;
        public CommandResult Result;
        public string CustomInfo;

        public Task<CommandResult> CurrentTask;
        public AbstractCommand Command;
        
    }

    public static class CommandInfoFactory
    {

        public static CommandInfo CreateCommandInfo(
            InteractionHandlerInfo handlerInfo,
            double signal)
        {
            CommandInfo commandInfo = new CommandInfo();
            commandInfo.Initiator = handlerInfo.Source;
            commandInfo.Signal = (float)signal;
            return commandInfo;
        }
    }


    public abstract class AbstractCommand : MonoBehaviour, IExecuteAsync, ICommandEventProvider, ISpeedProvider
    {
        public UnityEvent<CommandEvent> OnCommandEvent = new(); 

        UnityEvent<CommandEvent> ICommandEventProvider.GetCommandEvent() { return this.OnCommandEvent; }

        public List<AbstractCommandInfoModifier> CommandModifiers = new();
        public CommandBehaviourType BehaviourType;


        public List<CommandChainLink> CommandChainLinks = new();


        protected Task<CommandResult> _currentTask;

        public string CustomInfo;

        public virtual void Execute(CommandInfo commandInfo)
        {
            this.ExecuteAsync(commandInfo);
        }

        public Task<CommandResult> ExecuteAsync(CommandInfo commandInfo)
        {
            this.ApplyModifiers(ref commandInfo);
            return this._Run(commandInfo);
        }

        void ApplyModifiers(ref CommandInfo commandInfo)
        {
            foreach (var commandModifier in this.CommandModifiers)
            {
                commandModifier.Modify(ref commandInfo);
            }
        }

        async Task<CommandResult> _Run(CommandInfo commandInfo)
        {
            try
            {
                this._currentTask = this._Execute(commandInfo);

                this._BroadcastStart(commandInfo);
                var result = await this._currentTask;
                this._BroadcastEnd(commandInfo, result);

                this._CallChainLinks(commandInfo, result); // Chainlinks just get called directly and have no way to unsubscribe
                return result;
            }
            catch (System.Exception ex) 
            {
                Debug.LogError(ex);
                return new CommandResult
                { 
                    Type = CommandResultType.Interrupted
                };
            }
        }

        void _BroadcastStart(CommandInfo commandInfo)
        {
            Debug.Log($"Broadcast start".Pink());
            // broadcast command will start
            this.OnCommandEvent.Invoke(new CommandEvent
            {
                Type = CommandEventType.WillStart,
                Result = new CommandResult
                {
                    Type = CommandResultType.Pending,
                    CommandInfo = commandInfo
                },
                CustomInfo = this.CustomInfo,
                CurrentTask = this._currentTask,
                Command = this,
            });
        }

        void _BroadcastEnd(CommandInfo commandInfo, CommandResult commandResult)
        {
            Debug.Log($"did broadcast end for : {this.name}");
            // broadcast command is finished
            this.OnCommandEvent.Invoke(new CommandEvent
            {
                Type = CommandEventType.Ended,
                Result = commandResult,
                CustomInfo = this.CustomInfo,
                CurrentTask = null,
                Command = this,
            });
        }

        void _CallChainLinks(CommandInfo commandInfo, CommandResult commandResult)
        {
            foreach (var link in this.CommandChainLinks)
            {
                link.HandleCommandEnded(new CommandEvent
                {
                    Type = CommandEventType.Ended,
                    Result = commandResult,
                    CustomInfo = this.CustomInfo,
                });
            }
        }

        protected abstract Task<CommandResult> _Execute(CommandInfo commandInfo);
        public virtual double GetSpeed() { return 1d; }

    }




}


