using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Adapters
{

    public interface IInteractionHighlight
    {
        void Highlight(bool isOn);
    }

    public interface IOneAtATimeHighlightManager
    {
        void NextHighlight(IInteractionHighlight next);
        GameObject GetCurrent();
    }

    public interface ICamLockSessionOneAtATimeHighlightManager
    {
        void NextHighlight(IInteractionHighlight next);
    }

}