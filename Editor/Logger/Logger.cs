using UnityEngine;
using UnityEditor;
using System;

namespace DuksGames.Tools
{
    public static class Logger
    {
        public static void ImportLog(string s)
        {
            if (ArgonCachedPreferences.Preferences.IsImportLoggerEnabled)
                Debug.Log(s);
        }

        public static void ImportLogWarning(string s)
        {
            if (ArgonCachedPreferences.Preferences.IsImportLoggerEnabled)
                Debug.LogWarning(s);
        }
    }
}