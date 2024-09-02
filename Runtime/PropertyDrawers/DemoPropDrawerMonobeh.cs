using UnityEngine;

namespace DuksGames.Argon.PropertyDrawers
{
    public interface IDemoType
    {

    }

    public class DemoPropDrawerMonobeh : MonoBehaviour, IDemoType
    {
        [TypeEnforceAttribute(typeof(IDemoType))]
        public Component SillyProperty;

        [TypeEnforceAttribute(typeof(Camera))]
        public Component[] SillyArray;
    }
}