using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public class SwapMaterialKeySet : AbstractCustomPropKeySet<SwapMaterialObjectProcessor>
    {
        public override string TargetKey => "mel_material_list_marker";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.TargetKey;
        }
    }

    public class SwapMaterialObjectProcessor : AbstractCustomPropProcessor , 
        IApplyCustomProperties,
        IModelPostProcessor
    {

        [System.Serializable]
        class MaterialMapWrapper
        {
            public MaterialMapItem[] map; 
        }

        [System.Serializable]
        class MaterialMapItem
        {
            public string material;
            public string unityMaterial;
        }

        Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

        List<Material> _cachedSharedMaterials = new List<Material>();


        void AddMaterialFrom(string bleMaterialName, string swapToMaterialName) {
            var toMaterial = string.IsNullOrEmpty(swapToMaterialName) ? bleMaterialName : swapToMaterialName;
            var mat = MelGameObjectHelper.FindInProject<Material>(toMaterial, ".mat", false, "Material", $"ADD material fail");
            if (mat == null) {
                return;
            }

            this.materialMap.Add(bleMaterialName, mat);
        }

        public void Apply()
        {
            var ser = this.Config.getValue(this.KeySet.TargetKey);

            var wrapper = JsonUtility.FromJson<MaterialMapWrapper>(ser);
            foreach(var mm in wrapper.map) {
                this.AddMaterialFrom(mm.material, mm.unityMaterial);
            }
        }

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            foreach(var renderer in modelPostProcessInfo.Root.GetComponentsInChildren<Renderer>()) {
                if(renderer is ParticleSystemRenderer){
                    // ParticleSystemRenderer sharedMaterials are null apparently. 
                    continue; 
                }

                this._cachedSharedMaterials.Clear();
                renderer.GetSharedMaterials(this._cachedSharedMaterials);

                bool isSharedDirty = false;
                for (int i = 0; i < this._cachedSharedMaterials.Count; ++i) 
                {
                    var sharedMaterial = this._cachedSharedMaterials[i];
                    if (sharedMaterial == null) {
                        continue; // does this ever happen?
                    }
                    if (!this.materialMap.ContainsKey(sharedMaterial.name)) {
                        continue;
                    }
                    isSharedDirty = true;

                    this._cachedSharedMaterials[i] = this.materialMap[sharedMaterial.name];
                }

                if (isSharedDirty)
                {
                    renderer.sharedMaterials = this._cachedSharedMaterials.ToArray();
                }

            }
        }

    }
}