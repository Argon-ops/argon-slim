using UnityEngine;

namespace DuksGames.Argon.Adapters
{
    public interface ICamSwap
    {
        void SwapTo(Camera cam);
        void SwapFrom(Camera cam);
        void SwapToMain();
        Camera GetCurrent();
    }

    public interface ICamSwapManager
    {
        void AddManaged(Camera cam);
    }
}