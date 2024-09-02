using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{
    public class DestroyMessageReceiver : MonoBehaviour, ICustomDestroyMessageReceiver
    {
        public DestroyList DestroyList;

        public void PreCustomDestroyMessage()
        {
        }

        public void CustomDestroy()
        {
            this.DestroyList.DestroyMembers();
        }
    }
}