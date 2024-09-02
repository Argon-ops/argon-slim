using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Shared
{
    public static class TransformExtensions
    {

        public static IEnumerable<Transform> ImmediateChildren(this Transform t)
        {
            foreach (Transform c in t)
            {
                yield return c;
            }
        }

        public static IEnumerable<Transform> ParentsInclusive(this Transform t)
        {
            var tt = t;
            while (tt != null)
            {
                yield return tt;
                tt = tt.parent;
            }
        }

        /// <summary>
        /// returns one of the transform's local vectors based on index: 0 => right, 1 => up, 2 => forward
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns></returns>
        public static Vector3 GetLocalDirection(this Transform t, int index)
        {
            switch (index)
            {
                case 0: return t.right;
                case 1: return t.up;
                case 2: default: return t.forward;
            }
        }

         public static Vector3 GetLocalDirectionPositiveNegative(this Transform t, int index)
        {
            switch (index)
            {
                case 0: return t.right;
                case 1: return t.right * -1f;
                case 2: return t.up;
                case 3: return t.up * -1f;
                case 4: return t.forward;
                case 5: return t.forward * -1f;
                default: throw new System.ArgumentOutOfRangeException(); 
            }
        }


        /// <summary>
        /// Find the child, excluding this transform,
        ///  whose name matches exactName
        /// </summary>
        /// <param name="self"></param>
        /// <param name="exactName"></param>
        /// <returns></returns>
        public static Transform FindRecursive(this Transform self, string exactName)
                        => self.FindRecursive(child => child.name == exactName);

        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
        {
            var stack = new Stack<Transform>();
            stack.Push(self);
            while (stack.Count > 0)
            {
                var subject = stack.Pop();
                if (selector(subject)) 
                {
                    return subject;
                }
                for(int i=0; i<subject.childCount; ++i)
                {
                    stack.Push(subject.GetChild(i));
                }
            }
            return null;
            
        }

        public static Transform FindRecursiveSelfInclusive(this Transform self, string exactName)
                        => self.FindRecursiveSelfInclusive(child => child.name == exactName);


        public static Transform FindRecursiveSelfInclusive(this Transform self, Func<Transform, bool> selector)
        {
            if (selector(self))
            {
                return self;
            }

            foreach (Transform child in self)
            {
                var found = child.FindRecursiveSelfInclusive(selector);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public static Transform MakeChild(this Transform self, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(self);
            return go.transform;
        }

        public static T GetOrAddComponent<T>(this Transform self) where T : Component
        {
            var c = self.GetComponent<T>();
            if (c) { return c; }
            return self.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject g) where T : Component
        {
            return g.transform.GetOrAddComponent<T>();
        }


        /// <summary>
        /// Recursive version of GetComponent(string s)
        /// </summary>
        /// <param name="trans">The extended transform</param>
        /// <param name="typeName">The name of the type</param>
        /// <returns>A Component of the named type or null if none is found</returns>
        public static Component GetComponentInChildren(this Transform trans, string typeName) 
        {
            var found = trans.FindRecursiveSelfInclusive(ch => ch.GetComponent(typeName) != null);
            return found == null ? null : found.GetComponent(typeName);
        }

        public static T FindInAncestorSelfInclusive<T>(this Transform self) where T : Component
        {
            var subject = self;
            while (subject != null)
            {
                var result = subject.GetComponent<T>();
                if (result != null)
                    return result;
                subject = subject.parent;
            }

            return null;
        }

        public static string PathUpToButNotIncludingParent(this Transform self, Transform parent)
        {
            if(self == parent)
                return string.Empty;
                
            var result = self.name;
            var subject = self.parent;
            while(subject != parent)
            {
                result = $"{subject.name}/{result}";

                Assert.IsFalse(subject.parent == null, $"Asking for Path up to parent from {self.name} to {parent.name}. But {parent.name} isn't actually a parent of {self.name}");

                subject = subject.parent;
            }
            return result;
        }

    }
}