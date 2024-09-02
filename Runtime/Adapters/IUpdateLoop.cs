using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Adapters
{

    public interface IUpdateLoop
    {
        void DoIUpdateLoop();
    }

    public interface IUpdateStack
    {
        bool TakeOver(IUpdateLoop loop);
        bool Release(IUpdateLoop loop);
    }
}