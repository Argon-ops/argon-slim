using System.Linq;
using UnityEngine;

namespace DuksGames.Argon.Shared
{
    public static class ArrayHelper
    {

        public static bool BothNullOrSequenceEqual<T>(T[] a, T[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if ((a == null) != (b == null))
            {
                return false;
            }
            if (a == null)
            {
                return true;
            }

            return a.SequenceEqual(b);
        }
    }

}