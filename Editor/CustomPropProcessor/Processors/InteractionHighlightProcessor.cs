using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Core;
using DuksGames.Argon.Adapters;
using System;
using UnityEngine.Assertions;
using UnityEditor;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public class InteractionHighlightKeySet : AbstractCustomPropKeySet<InteractionHighlightProcessor>
    {
        public override string TargetKey => "mel_interaction_highlighter";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_highlight_mat");
            yield return this.AppendSuffix("_renderer_target");
            yield return this.AppendSuffix("_on_sleep_action");
            yield return this.AppendSuffix("_mode");
            yield return this.AppendSuffix("_click_beacon_prefab");
            yield return this.AppendSuffix("_set_initial_sleep_state");
            yield return this.AppendSuffix("_beacon_placement_option");
            yield return this.AppendSuffix("_beacon_nudge_vector");
            yield return this.AppendSuffix("_beacon_should_rotate_ninety");
            yield return this.AppendSuffix("_downtime_seconds");
            yield return this.AppendSuffix("_is_invisible_to_proximity");
            yield return this.AppendSuffix("_forward_face_reference");
            yield return this.AppendSuffix("_beacon_position_reference");
        }
    }

    public class InteractionHighlightProcessor : AbstractCustomPropProcessor, IModelPostProcessor, ILinkerProcess
    {

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            AbstractHighlighter abstractHighlighter;
            switch (this.GetIntWithSuffix("_mode"))
            {
                case 0: // highlight material
                default:
                    var ihighlighter = MelGameObjectHelper.AddIfNotPresent<InteractionHighlighter>(this.ApplyInfo.Target);
                    var matName = this.GetStringWithSuffix("_highlight_mat");
                    var highlightMat = MelGameObjectHelper.FindInProjectOrWarn<Material>(matName, "Material", $"Failed to find highlight material named: '{matName}' ");
                    ihighlighter.HighlightMat = highlightMat;

                    abstractHighlighter = ihighlighter;
                    break;

                case 1: // click beacon
                    var bhighlighter = MelGameObjectHelper.AddIfNotPresent<BeaconHighlighter>(this.ApplyInfo.Target);
                    var beaconPrefabName = this.GetStringWithSuffix("_click_beacon_prefab");

                    var builder = bhighlighter.gameObject.AddComponent<BeaconBuilder>();
                    builder.ClickBeaconPrefab = MelGameObjectHelper.FindGameObjectInProjectEnforceType<IBeaconDevice>(beaconPrefabName);
                    Assert.IsFalse(builder.ClickBeaconPrefab == null, $"Null beacon prefab for {this.ApplyInfo.Target.name}");
                    builder.beaconPlacementOption = (BeaconBuilder.BeaconPlacementOption)this.GetIntWithSuffix("_beacon_placement_option");
                    builder.normalizedBeaconNudge = this.GetVector3WithSuffix("_beacon_nudge_vector");
                    builder.beaconShouldRotateNinety = this.GetBoolWithSuffix("_beacon_should_rotate_ninety", true);

                    abstractHighlighter = bhighlighter;
                    break;

                case 2: // invisible
                    var invisibleHighlighter = MelGameObjectHelper.AddIfNotPresent<InvisibleHighlighter>(this.ApplyInfo.Target);
                    abstractHighlighter = invisibleHighlighter;
                    break;
            }

            abstractHighlighter.OnSleepDirective = (TurnOnOffDirectiveType)this.GetIntWithSuffix("_on_sleep_action", (int)TurnOnOffDirectiveType.TurnOff);

            var targetName = this.GetStringWithSuffix("_renderer_target");

            // Be very specific when finding the target renderer because of scenarios where there are two objects with the same name.
            //   (Not possible in Blender but possible in the imported fbx if there's an object with the same name as the blend's file name: "locker" and "locker.blend" for example)
            var tarRenderer = MelGameObjectHelper.FindInRootWithNameAndSelector(
                                        this.ApplyInfo.Target.transform, 
                                        targetName, 
                                        t => t.GetComponent<Renderer>() != null,
                                        modelPostProcessInfo.ImportHierarchyLookup);

            //Debug.Log($"LOOK UP contents: {modelPostProcessInfo.ImportHierarchyLookup.DDump()}".Pink());
            //Debug.Log($"InterHighlighter : tar renderer: {tarRenderer?.name}".Blue());
                                        
            abstractHighlighter.TargetRenderer = tarRenderer == null ? abstractHighlighter.gameObject : tarRenderer.gameObject;

            SetupSleepStateSetter.Setup(abstractHighlighter, this.ApplyInfo.Target, s => this.GetIntWithSuffix(s));

            modelPostProcessInfo.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter = abstractHighlighter;
        }

        public void Link(ModelPostProcessInfo mppi)
        {
            this.AddProximity(mppi.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter, mppi.ImportHierarchyLookup); // this.ApplyInfo.Target);
            this.SetBeaconPositionReference();
            this.AddDowntime();
            this.AddDestroyList();
        }

        void SetBeaconPositionReference()
        {
            var builder = this.ApplyInfo.Target.GetComponent<BeaconBuilder>();
            if(builder == null) 
            {
                return;
            }

            var referenceName = this.GetStringWithSuffix("_beacon_position_reference");
            if(string.IsNullOrEmpty(referenceName))
            {
                return;
            }
            var positioner = ShGameObjectHelper.FindInRoot(this.ApplyInfo.Target.transform, referenceName);
            Assert.IsFalse(positioner == null, $"Failed to find a beacon placement reference named: {referenceName}. on Target: {this.ApplyInfo.Target.name}");

            // create a new game object in case the original position object has a destroy tag
            var refPlacement = new GameObject($"BeaconPositionRef_");
            refPlacement.transform.SetParent(positioner.parent);
            refPlacement.transform.SetPositionAndRotation(positioner.transform.position, positioner.transform.rotation);
            builder.PositionReference = refPlacement.transform;
        }

        private void AddDestroyList()
        {
            var dl = this.ApplyInfo.Target.GetOrAddComponent<DestroyList>();
            var tar = this.ApplyInfo.Target;

            dl.Add(tar.GetComponent<ChoosableClickBeacon>());
            dl.Add(tar.GetComponent<GenericApparentPosition>());
            dl.Add(tar.GetComponent<FakeClickBeacon>());
            dl.Add(tar.GetComponent<SluggishProximity>());
            dl.Add(tar.GetComponent<HighlighterProximity>());
            dl.Add(tar.GetComponent<FakeIsAwakeProvider>());
            dl.Add(tar.GetComponent<HighlighterDowntime>());
        }

        void AddProximity(AbstractHighlighter highlighter, ImportHierarchyLookup ihl) // clickableGob) 
        {
            if (this.GetBoolWithSuffix("_is_invisible_to_proximity", true))
            {
                return;
            }

            var clickableGob = highlighter == null ? this.ApplyInfo.Target : highlighter.gameObject; // actually highlighter should never be null...
            var choosableClickBeacon = clickableGob.AddComponent<ChoosableClickBeacon>();

            // used for proximity and line of sight
            choosableClickBeacon.ILocatableBeaconLink = highlighter != null ? highlighter : clickableGob.AddComponent<GenericApparentPosition>();
            choosableClickBeacon.IClickBeaconLink = highlighter != null ? highlighter : clickableGob.AddComponent<FakeClickBeacon>();

            // fwd face 
            var forwardFaceRefName = this.GetStringWithSuffix("_forward_face_reference");
            var forwardFaceRef = MelGameObjectHelper.FindInRoot(this.ApplyInfo.Target.transform, forwardFaceRefName, ihl);
            if (forwardFaceRef != null)
            {
                choosableClickBeacon._HasForwardFaceVector = true;
                choosableClickBeacon.ForwardFaceVector = forwardFaceRef.gameObject.BoundsPositionOrTransformPosition()
                                                                        - this.ApplyInfo.Target.transform.position;
            }

            var sluggish = clickableGob.gameObject.AddComponent<SluggishProximity>();
            // TODO: enable radius
            var highprox = clickableGob.gameObject.AddComponent<HighlighterProximity>();
            highprox.Choosable = choosableClickBeacon;
            highprox.Highlighter = highlighter != null ? highlighter : clickableGob.AddComponent<FakeIsAwakeProvider>();
            sluggish.CallbackLink = highprox;
            // HighlighterProximity is the position provider also
            sluggish.SluggishPositionLink = highprox;
        }

        void AddDowntime()
        {
            var seconds = this.GetFloatWithSuffix("_downtime_seconds");
            if (seconds < .0001f) { return; }

            var highlighter = this.ApplyInfo.Target.GetComponent<AbstractHighlighter>();
            var handler = this.ApplyInfo.Target.GetComponent<ClickInteractionHandler>(); // TODO: mechanism so that the user can specify a different handler obj if they want

            if (!handler)
            {
                Debug.LogWarning($"Unable to link highlighter {highlighter.gameObject.name} to a handler");
                return;
            }

            var downtime = this.ApplyInfo.Target.gameObject.AddComponent<HighlighterDowntime>();
            downtime.Handler = handler;
            downtime.Highlighter = highlighter;
            downtime.Downtime = seconds;
        }
    }
}