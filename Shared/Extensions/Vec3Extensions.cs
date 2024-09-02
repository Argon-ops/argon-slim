using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Shared
{
    public static class Vec3Extensions
    {

        public static int LargestComponent(this Vector3 v)
        {
            return v.x > v.y ? (v.x > v.z ? 0 : 2) : (v.y > v.z ? 1 : 2);
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static float SumOfComponents(this Vector3 v)
        {
            return v.x + v.y + v.z;
        }

        public static bool EqualsEpsilonEMinus3(this Vector3 lhs, Vector3 rhs)
        {
            float num = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return num4 < 29.9999994E-7f; 
        }

        public static bool EqualsEpsilonEMinus5(this Vector3 v, Vector3 b)
        {
            // actually Unity's built in equality is overloaded as epsilon=1e-5
            // and .Equals is 'perfect' epsilon=0 equality
            return v == b; 
        }

        public static bool EqualsEpsilon(this Vector3 lhs, Vector3 rhs, float epsilon)
        {
            float num = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return num4 < 3f * epsilon * epsilon; 
        }

        
    }
}