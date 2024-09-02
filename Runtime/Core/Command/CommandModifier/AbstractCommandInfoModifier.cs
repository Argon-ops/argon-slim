using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{
    [System.Serializable]
    public abstract class AbstractCommandInfoModifier : MonoBehaviour
    {
        public void Modify(ref CommandInfo commandInfo)
        {
            this._Modify(ref commandInfo);
        }

        protected abstract void _Modify(ref CommandInfo commandInfo);
    }

}