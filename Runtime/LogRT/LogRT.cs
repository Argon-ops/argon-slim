using UnityEngine;

namespace DuksGames.Argon.LogRT
{

#if UNITY_EDITOR
    public static class LogRT
    {
        static bool _isLoggingEnabled => true; // TODO: make this a setting
        
        public static void Log(string message, Object context = null)
        {
            if (!_isLoggingEnabled) return;
            Debug.Log(message, context);
        }

        public static void LogWarning(string message, Object context = null)
        {
            if (!_isLoggingEnabled) return;
            Debug.LogWarning(message, context);
        }

        public static void LogError(string message, Object context = null)
        {
            if (!_isLoggingEnabled) return;
            Debug.LogError(message, context);
        }
    }
#else
    public static class LogRT
    {
        public static void Log(string message, Object context = null){}

        public static void LogWarning(string message, Object context = null){}

        public static void LogError(string message, Object context = null){}
    }
#endif
}