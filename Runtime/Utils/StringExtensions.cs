
using UnityEngine;

namespace DuksGames.Argon.Utils
{

    public static class StringExtensions
    {
        public static string Color(this string s, Color c)
        {
            return $"<color='#{ColorUtility.ToHtmlStringRGB(c)}'>{s}</color>";
        }

        public static string Pink(this string s)
        {
            return s.Color(new Color(1f, .5f, .5f));
        }

        public static string Blue(this string s)
        {
            return s.Color(new Color(.5f, .7f, 1f));
        }

        public static string Yellow(this string s)
        {
            return s.Color(new Color(.9f, 1f, .2f));
        }

        public static string Green(this string s)
        {
            return s.Color(new Color(.2f, 1f, .3f));
        }

        public static string Turquoise(this string s)
        {
            return s.Color(new Color(.3f, 1f, 1f));
        }

        public static string Orange(this string s)
        {
            return s.Color(new Color(.8f, .3f, .1f));
        }
    }
}