using UnityEngine;

namespace DuksGames.LogicBlocks
{
    public interface IOnOffStateProvider
    {
        bool IsOn();
    }
    
    public abstract class AbstractOnOffStateProvider : MonoBehaviour , IOnOffStateProvider
    {
        public abstract bool IsOn();
    }
    
}