using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{

    public interface IPlayInstructionProvider
    {
        PlayInstruction GetPlayInstructions(CommandInfo info);
    }

    public class ContinueFromPlayInstructionGenerator : MonoBehaviour, IPlayInstructionProvider
    {
        public PlayInstruction GetPlayInstructions(CommandInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}