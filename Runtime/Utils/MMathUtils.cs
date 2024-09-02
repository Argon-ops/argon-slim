using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuksGames.FPS.Utils
{
    public static class MMathUtils
    {

        public static void CopyVec(Vector3 from, Vector3 to)
        {
            to.Set(from.x, from.y, from.z);
        }

        // TODO: ToRef version of this function
        public static Quaternion FilterBlenderXRot(Quaternion q)
        {
            // Be suspicious of rotations around x by 90 degrees
            var x = q.eulerAngles.x;
            if (q.eulerAngles.x > 89.7 && q.eulerAngles.x < 90.3)
            {
                x = 0;
            }
            return Quaternion.Euler(x, q.eulerAngles.y, q.eulerAngles.z);
        }

        public static Quaternion IsolateYRot(Quaternion q)
        {
            return Quaternion.Euler(0, q.eulerAngles.y, 0);
        }

        public static int GreatestComponentIndex(Vector3 vector3)
        {
            if (vector3.x > vector3.y)
            {
                return vector3.x > vector3.z ? 0 : 2;
            }
            return vector3.y > vector3.z ? 1 : 2;
        }

        public static Vector3 BasisForIndex(int i)
        {
            switch (i)
            {
                case 0:
                default:
                    return Vector3.right;
                case 1:
                    return Vector3.up;
                case 2:
                    return Vector3.forward;
            }
        }

        public static Color32 ToColor32(Color a)
        {
            return new Color32(
                (byte)(a.r * 256),
                (byte)(a.g * 256),
                (byte)(a.b * 256),
                (byte)(a.a * 256));
        }

        public static bool EqualF(float a, float b, float eps = .00001f)
        {
            return Mathf.Abs(a - b) < eps;
        }

        public static bool ColorEquals(Color a, Color b, float eps = .00001f)
        {
            return
                EqualF(a.r, b.r, eps) &&
                EqualF(a.g, b.g, eps) &&
                EqualF(a.b, b.b, eps) &&
                EqualF(a.a, b.a, eps);
        }

        public static void PlaceOnTop(Collider collider, Transform pedestal, float buffer = 1.1f)
        {
            if (collider == null) { return; }

            var targetPos = pedestal.position;
            targetPos.y += buffer * collider.bounds.extents.y;
            collider.transform.position = targetPos;
        }


        public static Vector3 NearestPointOnPlane(Vector3 p, Vector3 normal, Vector3 anyAlreadyOnPlane)
        {
            return p - Vector3.Dot(normal, (p - anyAlreadyOnPlane)) * normal;
        }

        public static float RandNegPosOne() { return Random.Range(-1f, 1f); }

        public static Vector3 RandNegPosV3()
        {
            return new Vector3(
                RandNegPosOne(),
                RandNegPosOne(),
                RandNegPosOne());
        }

        public static void AssertNoNan(Vector3 v)
        {
            for (int i = 0; i < 3; ++i)
            {
                Assert.IsFalse(float.IsNaN(v[i]), $"nan val at {i}");
            }
        }


    }
}