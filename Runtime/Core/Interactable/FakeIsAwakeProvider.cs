using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class FakeIsAwakeProvider : MonoBehaviour, IIsAwakeProvider
    {
        public bool GetIsAwake()
        {
            return true;
        }
    }
}