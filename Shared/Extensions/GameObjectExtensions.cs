using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DuksGames.Argon.Shared
{
    public static class GameObjectExtensions
    {
        static Bounds nullBounds = new Bounds();
        public static bool FindBounds(this GameObject g, out Bounds bounds)
        {
            if (g.GetComponent<Collider>())
            {
                bounds = g.GetComponent<Collider>().bounds;
                return true;
            }
            if (g.GetComponent<Renderer>())
            {
                bounds = g.GetComponent<Renderer>().bounds;
                return true;
            }
            bounds = nullBounds;
            return false;
        }

        /// <summary>
        /// If this game object has an attached Collider 
        /// or Renderer return the bounds center of that Collider or Renderer.
        /// Otherwise, return its transform's position.
        /// </summary>
        public static Vector3 BoundsPositionOrTransformPosition(this GameObject g)
        {
            Bounds b;
            if (g.FindBounds(out b))
            {
                return b.center;
            }
            return g.transform.position;
        }
    }
}