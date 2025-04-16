using UnityEngine;

namespace DuksGames.Argon.RTLog
{
    public static class RTLog
    {
        public static void Log(string message, Object context = null)
        {
            Debug.Log(message, context);
        }

        public static void LogWarning(string message, Object context = null)
        {
            Debug.LogWarning(message, context);
        }

        public static void LogError(string message, Object context = null)
        {
            Debug.LogError(message, context);
        }
    }
}