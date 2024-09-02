using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Event
{
    public struct PlayInstruction
    {
        public float Direction;
        public float FromA;
        public float ToB;
        public bool IsLooping;
        public bool IsValid;
    }

    public static class PlayInstructionFactory
    {
        public static PlayInstruction Forwards01(float ToB = 1f, bool IsLooping = false)
        {
            return new PlayInstruction
            {
                Direction = 1f,
                FromA = 0f,
                ToB = ToB,
                IsLooping = IsLooping,
                IsValid = true
            };
        }
    }

    [System.Serializable]
    public struct CommandInfo
    {
        /// <summary>
        /// The gameobject whose component initiated this command. For example, the player.
        /// </summary>
        public GameObject Initiator;

        /// <summary>
        /// A message to pass from command issuer to command and between chained commands.
        /// </summary>
        public float Signal;

    }

    public interface IExecute
    {
        void Execute(CommandInfo commandInfo);
    }

    public interface IExecuteAsync : IExecute
    {
        Task<CommandResult> ExecuteAsync(CommandInfo commandInfo);
    }


    public enum CommandResultType
    {
        Pending, NeverStarted, Interrupted, Completed
    }

    public struct CommandResult
    {
        public CommandInfo CommandInfo;
        public CommandResultType Type;

        public bool IsValid() { return this.Type != CommandResultType.Pending; }

        public static CommandResult CompletedResult(CommandInfo commandInfo)
        {
            return new CommandResult
            {
                CommandInfo = commandInfo,
                Type = CommandResultType.Completed
            };
        }

        public static CommandResult NeverStarted(CommandInfo commandInfo)
        {
            return new CommandResult
            {
                CommandInfo = commandInfo,
                Type = CommandResultType.NeverStarted
            };
        }
    }

    public enum CommandEventType
    {
        WillStart, Interrupted, Ended
    }

}