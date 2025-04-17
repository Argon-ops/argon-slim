using UnityEngine;
using UnityEditor;
using System;

namespace DuksGames.Tools
{
    public static class Logger
    {
        static bool _isImportLoggerEnabled => true;  
        
        public static void Log(string s)
        {
            if(_isImportLoggerEnabled)
                Debug.Log(s);
        }

        public static void LogWarning(string s)
        {
            if(_isImportLoggerEnabled)
                Debug.LogWarning(s);
        }

        public static void LogError(string s)
        {
            if(_isImportLoggerEnabled)
                Debug.LogError(s);
        }
    }
}