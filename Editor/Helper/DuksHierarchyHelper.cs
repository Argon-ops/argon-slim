using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using DuksGames.Argon.Shared;
using DuksGames.Argon.Animate;

namespace DuksGames.Tools
{
    public static class DuksCommandObjects
    {
        public static string DuksSpecialsRootName = "z_Argon_z";
        public static string CommandRootName = "CommandRoot";

        static GameObject GetRoot(Transform root) {
            return MelGameObjectHelper.GetChildOfOrCreate(DuksCommandObjects.DuksSpecialsRootName, root);
        }

        public static GameObject GetCommandRoot(Transform root) {
            return MelGameObjectHelper.GetChildOfOrCreate(CommandRootName, GetRoot(root).transform);
        }

        private static GameObject CreateCommandChild(Transform root, string _name) {
            var commandRoot = GetCommandRoot(root);
            var child = new GameObject(MelGameObjectHelper.GetUnusedChildName(_name, commandRoot));
            child.transform.SetParent(commandRoot.transform);
            return child;
        }

        public static GameObject FindOrCreateCommandHolder(Transform sceneRoot, string _name) 
        {
            Assert.IsFalse(sceneRoot == null, $"null scene root");

            var commandRoot = DuksCommandObjects.GetCommandRoot(sceneRoot);

            Assert.IsFalse(commandRoot == null, $"Null commandRoot with sceneRoot {sceneRoot.name}");

            var commandHolder = commandRoot.transform.Find(_name);
            if (commandHolder != null) { 
                return commandHolder.gameObject; 
            
            }
            return DuksCommandObjects.CreateCommandChild(sceneRoot, _name);
        }

        public static Transform GetPCWRoot(Transform sceneRoot) {
            var root = DuksCommandObjects.GetRoot(sceneRoot);
            return MelGameObjectHelper.GetChildOfOrCreate("ClipWrapperRoot", root.transform).transform;
        }

        public static GameObject CreatePCWHolder(Transform sceneRoot, string _name) {
            var pcwRoot = DuksCommandObjects.GetPCWRoot(sceneRoot);
            var uniqueName = MelGameObjectHelper.GetUnusedChildName(_name, pcwRoot.gameObject);
            return MelGameObjectHelper.CreateChild(uniqueName, pcwRoot);
        }

        
    }

    public static class DuksStrangePathConverter
    {
        public static string StrangePathToPath(string strangePath)
        {
            if (strangePath == null) { return ""; }
            var split = strangePath.Split("~~&&&~~"); 
            return string.Join("/", split);
        }

        public static IEnumerable<string> ConvertStrangePathsToPath(string[] strangePaths)
        {
            return strangePaths.Select(s => DuksStrangePathConverter.StrangePathToPath(s));
        }
    }
}