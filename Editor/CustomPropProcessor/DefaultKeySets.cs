using UnityEngine;
using UnityEditor;

namespace DuksGames.Tools
{
    public class DefaultKeySets
    {
        public IProcessorKeySet[] KeySets = new IProcessorKeySet[] {
            ScriptableObject.CreateInstance<BoxColliderInterpreterKeySet>(),
            ScriptableObject.CreateInstance<LayerInterpreterKeySet>(),
            ScriptableObject.CreateInstance<MeshColliderKeySet>(),
            ScriptableObject.CreateInstance<NoRendererKeySet>(),
            ScriptableObject.CreateInstance<OffMeshLinkKeySet>(),
            ScriptableObject.CreateInstance<ReplaceWithPrefabKeySet>(),
            ScriptableObject.CreateInstance<RigidbodyKeySet>(),
            ScriptableObject.CreateInstance<StaticFlagsKeySet>(),
            ScriptableObject.CreateInstance<TagParseKeySet>(),
            ScriptableObject.CreateInstance<ActionStarterKeySet>(),
            ScriptableObject.CreateInstance<ClickInteractableProcessorKeySet>(),
            ScriptableObject.CreateInstance<InteractionHighlightKeySet>(),
            ScriptableObject.CreateInstance<DestroyKeySet>(),
            ScriptableObject.CreateInstance<ParticleSystemKeySet>(),
            ScriptableObject.CreateInstance<VisualEffectKeySet>(),
            ScriptableObject.CreateInstance<ObjectEnableKeySet>(),
            ScriptableObject.CreateInstance<ScreenOverlayEnableKeySet>(),
            ScriptableObject.CreateInstance<AudioEnableKeySet>(),
            ScriptableObject.CreateInstance<SliderColliderKeySet>(),
            ScriptableObject.CreateInstance<ComponentByNameKeySet>(),
            ScriptableObject.CreateInstance<SpawnerKeySet>(),
            ScriptableObject.CreateInstance<CamLockSessionEnableKeySet>(),
            ScriptableObject.CreateInstance<DisableComponentKeySet>(),
            ScriptableObject.CreateInstance<ComponentEnableKeySet>(),
            ScriptableObject.CreateInstance<TextMeshKeySet>(),
            ScriptableObject.CreateInstance<RETwoPickupSessionKeySet>(),
            ScriptableObject.CreateInstance<GlobalSettingsKeySet>(),
            ScriptableObject.CreateInstance<ForcePCWKeySet>(),
            ScriptableObject.CreateInstance<SwapMaterialKeySet>(),
            ScriptableObject.CreateInstance<PlayableScalarAdapterKeySet>(),
            ScriptableObject.CreateInstance<SwapMaterialEnableKeySet>(),
            ScriptableObject.CreateInstance<CamSwapManagedKeySet>(),
        };

        public IProcessorKeySet[] UniquePerObjectKeySets = new IProcessorKeySet[] {
            ScriptableObject.CreateInstance<ApplyClassWithCustomDataKeySet>(),
        };

    }
}