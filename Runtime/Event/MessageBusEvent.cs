using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Event
{
    public struct MessageBusEvent
    {
        public string Type;
        public GameObject[] Targets;
        public CommandInfo CommandInfo;
        public string CustomInfo;

    }
}