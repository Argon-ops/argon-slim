using UnityEngine;

namespace DuksGames.Argon.PropertyDrawers
{
    public class TypeEnforceAttribute : PropertyAttribute
    {
        public System.Type Type;
        public bool AllowSceneObjects = true;

        public TypeEnforceAttribute(System.Type _Type, bool allowSceneObjects = true)
        {
            this.Type = _Type;
            this.AllowSceneObjects = allowSceneObjects;
        }
    }
}