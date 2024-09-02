using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using DuksGames.Argon.Shared;

// TODO: draw lots of widgets if show pref: show Argon Widgets is on

namespace DuksGames.Tools
{
    public class OffMeshLinkKeySet : AbstractCustomPropKeySet<OffMeshLinkParser>
    {
        private static string[] Suffixes = new string[] {
            "_setup_type",
            "_start",
            "_end",
            "_cost_override",
            "_bidirectional",
            "_activated",
            "_auto_update_positions",
            "_navigation_area" };

        public override string TargetKey => "mel_off_mesh_link";

        public override IEnumerable<string> GetKeys()
        {
            foreach (var suffix in OffMeshLinkKeySet.Suffixes)
            {
                yield return this.AppendSuffix(suffix);
            }
        }
    }

    public class OffMeshLinkParser : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        void ApplyLongAxis(OffMeshLink oml)
        {
            var filter = oml.GetComponent<MeshFilter>();
            int longAxisIdx = filter.sharedMesh.bounds.size.LargestComponent();
            var longAxisWorldSpace = filter.transform.GetLocalDirection(longAxisIdx) * filter.sharedMesh.bounds.size[longAxisIdx] / 2f;

            var endWorld = oml.transform.TransformPoint(filter.sharedMesh.bounds.center) + longAxisWorldSpace;
            var startWorld = oml.transform.TransformPoint(filter.sharedMesh.bounds.center) + longAxisWorldSpace * -1f;

            var end = MelGameObjectHelper.CreateNewAt(endWorld, oml.gameObject);
            var start = MelGameObjectHelper.CreateNewAt(startWorld, oml.gameObject);

            oml.startTransform = start.transform;
            oml.endTransform = end.transform;
        }

        void ApplyManual(OffMeshLink oml)
        {
            var end = MelGameObjectHelper.FindInScene(this.GetStringWithSuffix("_end"));
            var start = MelGameObjectHelper.FindInScene(this.GetStringWithSuffix("_start"));

            try
            {
                // Make new empties at end pos / start pos; otherwise assignment doesn't work
                var endl = MelGameObjectHelper.CreateNewAt(end.transform.position, oml.gameObject);
                var startl = MelGameObjectHelper.CreateNewAt(start.transform.position, oml.gameObject);
                oml.startTransform = startl.transform;
                oml.endTransform = endl.transform;

            }
            catch (System.NullReferenceException nre)
            {
                Debug.LogError($"There was a problem trying to find: {this.GetStringWithSuffix("_end")} and/or {this.GetStringWithSuffix("_start")}");
                throw nre;
            }
        }

        void ApplyStartEnd(OffMeshLink oml)
        {
            if (this.GetIntWithSuffix("_setup_type") == 0)
            {
                this.ApplyLongAxis(oml);
                return;
            }
            this.ApplyManual(oml);
        }

        void ApplyOptions(OffMeshLink oml)
        {
            oml.costOverride = this.GetIntWithSuffix("_cost_override");
            oml.biDirectional = this.GetBoolWithSuffix("_bidirectional");
            oml.activated = this.GetBoolWithSuffix("_activated");
            oml.autoUpdatePositions = this.GetBoolWithSuffix("_auto_update_positions");
            oml.area = NavMesh.GetAreaFromName(this.GetStringWithSuffix("_navigation_area"));
        }

        public void Apply()
        {
            var oml = MelGameObjectHelper.AddIfNotPresent<OffMeshLink>(ApplyInfo.Target);
            this.ApplyOptions(oml);
            this.ApplyStartEnd(oml);
        }



    }
}