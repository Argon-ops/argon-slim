using DuksGames.Argon.Adapters;
using UnityEngine;

namespace DuksGames.Argon.Gameplay
{
    public class PlayerProvider : MonoBehaviour, IPlayerProvider
    {
        [SerializeField] GameObject _player;
        public GameObject GetPlayer()
        {
            return this._player;
        }
    }
}