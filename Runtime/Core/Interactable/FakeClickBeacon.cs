using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class FakeClickBeacon : MonoBehaviour, IClickBeacon
    {
        public void SetState(EClickBeaconState mode)
        {
        }
    }
}