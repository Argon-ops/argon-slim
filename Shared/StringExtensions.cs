using System;

namespace DuksGames.Argon.Shared
{
    public static class StringExtensions
    {
        public static string SubstringRangeSafe(this string s, int startIndex, int length)
        {
            try
            {
                return s.Substring(startIndex, length);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (startIndex < s.Length)
                {
                    return s.Substring(System.Math.Max(startIndex, 0));
                }
                return "";
            }
        }

        public static string SubstringFromEndRangeSafe(this string s, int numCharsFromEnd)
        {
            try
            {
                return s.Substring(s.Length - numCharsFromEnd);
            }
            catch (ArgumentOutOfRangeException)
            {
                return s;
            }
        }

        public static T ToEnum<T>(this string s) where T : struct, Enum
        {
            if (Enum.TryParse<T>(s, out T result))
                return result;

            throw new ArgumentException($"Enum parse failed with string: {s}. Type {typeof(T).FullName}");
        }


        public static string[] SplitOverCommas(this string s)
        {
            return s.Split(',');
        }


    }
}