using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Adapters
{
    public interface IPlayerProvider
    {
        GameObject GetPlayer();
    }

    public interface IInventoryManager
    {
        bool Acquire(GameObject item);
        int Contains(string itemName);
        GameObject Provide(string itemName);
        bool Remove(string itemName);
    }
}