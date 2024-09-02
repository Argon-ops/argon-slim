using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Gameplay
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float seconds;

        void Start()
        {
            Destroy(this.gameObject, this.seconds);
        }
    }
}