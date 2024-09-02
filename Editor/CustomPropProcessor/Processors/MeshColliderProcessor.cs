using UnityEngine;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class MeshColliderKeySet : AbstractCustomPropKeySet<MeshColliderProcessor>
    {
        public override string TargetKey => "mel_mesh_collider";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_convex");
            yield return this.AppendSuffix("_is_trigger");
            yield return this.AppendSuffix("_cooking_options");
            yield return this.AppendSuffix("_material");
        }
    }

    public class MeshColliderProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        void MakeMeshReadable_ButThisDoesntWork()
        {
            // In certain cases (e.g. you changed the vertices of a mesh that has a  
            //  mesh-collider. And its import is associated with a prefab/prefab-variant)
            //  This error throws on re-import: 
            //     This Mesh Collider is attached to GameObject at path 'MeshColliderDemo_PF_TEST_DLME/MarbleCourse.003' with Mesh 'MarbleCourse.003' in Scene 'TestPF_Import_Styles_DELME' but doesn't have Read/Write enabled. Only colliders that have default cooking options, are not scaled non-uniformly, and their meshes are not marked dirty can leave this disabled.
            //  Unity needs the import Mesh to be read-write enabled. 
            //  This keeps the mesh's data in memory addressable on the cpu side. To free up this memory we should call mesh.UploadMeshData(true)
            this.ApplyInfo.GetModelImporter().isReadable = true;
        }

        public void Apply()
        {
            // this.MakeMeshReadable_ButThisDoesntWork(); // Don't think we can call here. probably need to set this and then call assetImporter.SaveAndReimport
            var meshCollider = MelGameObjectHelper.AddIfNotPresent<MeshCollider>(ApplyInfo.Target);
            meshCollider.convex = this.GetBoolWithSuffix("_convex");
            meshCollider.isTrigger = this.GetBoolWithSuffix("_is_trigger");
            meshCollider.cookingOptions = (MeshColliderCookingOptions)this.GetValueWithSuffix<int>("_cooking_options");
            meshCollider.sharedMaterial = MelGameObjectHelper.FindInProject<PhysicMaterial>(this.GetValueWithSuffix<string>("_material"));
        }
    }
}