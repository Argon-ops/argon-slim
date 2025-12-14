using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public class BoxColliderInterpreterKeySet : AbstractCustomPropKeySet<BoxColliderProcessor>
    {
        public override string TargetKey => "mel_box_collider";
        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_is_trigger");
            yield return this.AppendSuffix("_material");
            yield return this.AppendSuffix("_scale_dimensions");
        }
    }

    public class BoxColliderProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var meshFilter = this.ApplyInfo.Target.GetComponent<MeshFilter>();
            var boxCollider = MelGameObjectHelper.AddIfNotPresent<BoxCollider>(this.ApplyInfo.Target);
            var scaleDimensions = this.GetVector3WithSuffix("_scale_dimensions", false, Vector3.one);
            scaleDimensions = scaleDimensions.Abs().SumOfComponents() < Mathf.Epsilon ? Vector3.one : scaleDimensions;
            boxCollider.size = Vector3.Scale(meshFilter.sharedMesh.bounds.size, scaleDimensions);
            boxCollider.center = meshFilter.sharedMesh.bounds.center;
            boxCollider.isTrigger = this.GetBoolWithSuffix("_is_trigger");
            boxCollider.sharedMaterial = MelGameObjectHelper.FindInProjectOrWarn<PhysicsMaterial>(
                                        this.GetValueWithSuffix<string>("_material"));
        }
    }
}