using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class RigidbodyKeySet : AbstractCustomPropKeySet<RigidbodyParser>
    {
        public override string TargetKey => "mel_rigidbody";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_mass");
            yield return this.AppendSuffix("_drag");
            yield return this.AppendSuffix("_angular_drag");
            yield return this.AppendSuffix("_use_gravity");
            yield return this.AppendSuffix("_is_kinematic");
            yield return this.AppendSuffix("_interpolate");
            yield return this.AppendSuffix("_collision_detection");
            yield return this.AppendSuffix("_freeze_position");
            yield return this.AppendSuffix("_freeze_rotation");
        }
    }

    public class RigidbodyParser : AbstractCustomPropProcessor, IApplyCustomProperties
    {

        int getPositionConstraints(string key)
        {
            dynamic fff = this.Config.lookup[key];
            return (fff[0] > 0f ? (int)RigidbodyConstraints.FreezePositionX : 0) |
                (fff[1] > 0f ? (int)RigidbodyConstraints.FreezePositionY : 0) |
                (fff[2] > 0f ? (int)RigidbodyConstraints.FreezePositionZ : 0);
        }

        int getRotationConstraint(string key)
        {
            dynamic fff = this.Config.lookup[key];
            return (fff[0] > 0f ? (int)RigidbodyConstraints.FreezeRotationX : 0) |
                (fff[1] > 0f ? (int)RigidbodyConstraints.FreezePositionY : 0) |
                (fff[2] > 0f ? (int)RigidbodyConstraints.FreezeRotationZ : 0);
        }

        public void Apply()
        {
            var rb = MelGameObjectHelper.AddIfNotPresent<Rigidbody>(ApplyInfo.Target);

            rb.mass = this.Config.getFloat(this.AppendSuffix("_mass"));
            rb.drag = this.Config.getFloat(this.AppendSuffix("_drag"));
            rb.angularDrag = this.Config.getFloat(this.AppendSuffix("_angular_drag"));
            rb.useGravity = this.GetBoolWithSuffix("_use_gravity");
            rb.isKinematic = this.GetBoolWithSuffix("_is_kinematic");
            rb.interpolation = (RigidbodyInterpolation)this.GetIntWithSuffix("_interpolate");
            rb.collisionDetectionMode = (CollisionDetectionMode)this.GetIntWithSuffix("_collision_detection");
            rb.constraints = (RigidbodyConstraints)(this.getPositionConstraints(this.AppendSuffix("_freeze_position")) |
                                                    this.getRotationConstraint(this.AppendSuffix("_freeze_rotation")));
        }

    }
}