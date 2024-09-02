// using UnityEngine;
// using UnityEditor;
// using System.Collections.Generic;
// using System;

// namespace DuksGames.Tools
// {
//     public static class TransformExtensions
//     {

//         public static IEnumerable<Transform> ImmediateChildren(this Transform t) {
//             foreach(Transform c in t) {
//                 yield return c;
//             }
//         }

//         public static IEnumerable<Transform> ParentsInclusive(this Transform t) {
//             var tt = t;
//             while(tt != null) {
//                 yield return tt;
//                 tt = tt.parent;
//             }
//         }

//         /// <summary>
//         /// returns one of the transform's local vectors based on index: 0 => right, 1 => up, 2 => forward
//         /// </summary>
//         /// <param name="index">the index</param>
//         /// <returns></returns>
//         public static Vector3 GetLocalDirection(this Transform t, int index) {
//             switch (index) {
//                 case 0: return t.right;
//                 case 1: return t.up;
//                 case 2: default: return t.forward;
//             }
//         }

//         /// <summary>
//         /// Find the child, excluding this transform,
//         ///  whose name matches exactName
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="exactName"></param>
//         /// <returns></returns>
//         public static Transform FindRecursive(this Transform self, string exactName) 
//                         => self.FindRecursive(child => child.name == exactName);

//         public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
//         {

//             foreach (Transform child in self)
//             {
//                 if(selector(child)) {
//                     return child;
//                 }

//                 var found = child.FindRecursive(selector);

//                 if (found != null)
//                 {
//                     return found;
//                 }
//             }

//             return null;
//         }

//         public static Transform FindRecursiveSelfInclusive(this Transform self, string exactName) 
//                         => self.FindRecursiveSelfInclusive(child => child.name == exactName);


//         public static Transform FindRecursiveSelfInclusive(this Transform self, Func<Transform, bool> selector)
//         {
//             if(selector(self)) {
//                 return self;
//             }

//             foreach (Transform child in self)
//             {
//                 var found = child.FindRecursiveSelfInclusive(selector);

//                 if (found != null)
//                 {
//                     return found;
//                 }
//             }

//             return null;
//         }

//         public static Transform MakeChild(this Transform self, string name) {
//             var go = new GameObject(name);
//             go.transform.SetParent(self);
//             return go.transform;
//         }

//         public static T GetOrAddComponent<T>(this Transform self) where T : Component {
//             var c = self.GetComponent<T>();
//             if (c) { return c; }
//             return self.gameObject.AddComponent<T>();
//         }

//         public static T GetOrAddComponent<T>(this GameObject g) where T : Component {
//             return g.transform.GetOrAddComponent<T>();
//         }

//     }
// }