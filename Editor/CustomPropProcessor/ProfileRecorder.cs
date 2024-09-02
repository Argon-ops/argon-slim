using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Profiling;

namespace DuksGames.Tools
{
    public class ProfileRecorder
    {
        public static void Start(string profileName)
        {
            string s = $"Profiler/{profileName}";
            Directory.CreateDirectory("Profiler");
            Profiler.logFile = s;
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;
            Profiler.BeginSample($"Import {profileName}");
        }

        public static void End()
        {
            Profiler.EndSample();
            Profiler.enabled = false;
            Profiler.logFile = "";
        }
    }
}