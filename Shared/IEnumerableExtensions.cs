
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DuksGames.Argon.Shared
{
    public static class IEnumerableExtensions
    {

        public static string JoinSelf<T>(this IEnumerable<T> enumerable, System.Func<T, string> convert, string sep = ", ")
        {
            return string.Join(sep, enumerable.Select(convert));
        }

        public static string JoinSelf(this IEnumerable<string> enumerable, string sep = ", ")
        {
            return string.Join(sep, enumerable);
        }

        public static string JoinSelf(this IEnumerable<Transform> enumerable, string sep = ", ")
        {
            return enumerable.JoinSelf(t => t != null ? t.name : "", sep);
        }

        public static string JoinSelf(this IEnumerable<GameObject> enumerable, string sep = ", ")
        {
            return enumerable.JoinSelf(g => g != null ? g.name : "", sep);
        }

    }
}