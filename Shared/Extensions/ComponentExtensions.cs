using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DuksGames.Argon.Shared
{
    public static class ComponentExtensions
    {
        public static T CastOrGet<T>(this Component c)
        {
            if (c is T targetType)
            {
                return targetType;
            }
            return c.gameObject.GetComponent<T>();
        }

        public static IEnumerable<T> CastOrGet<T>(this Component[] components)
        {
            return components.Select(c => c.CastOrGet<T>());
        }
    }
}