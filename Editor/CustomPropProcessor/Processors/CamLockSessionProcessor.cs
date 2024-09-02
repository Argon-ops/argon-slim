using System.Collections.Generic;
using DuksGames.Argon.Core;
using UnityEngine;

namespace DuksGames.Tools
{


    public class CamLockSessionEnableKeySet : EnableableKeySet<CamLockSessionEnableProcessor> //  AbstractCustomPropKeySet<CamLockSessionEnableProcessor>
    {
        public override string TargetKey => "mel_cam_lock_session_enable";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_release_cursor");
            yield return this.AppendSuffix("_show_root_object");
            yield return this.AppendSuffix("_hide_root_object");
        }
    }

    public class CamLockSessionEnableProcessor : EnableablePropProcessor, IModelPostProcessor
    {

        public void PostProcessModel(ModelPostProcessInfo modelPostProcessInfo)
        {
            var camLockSession = this.ApplyInfo.Target.AddComponent<CamLockSessionEnable>();
            camLockSession.ShouldReleaseCursor = this.GetBoolWithSuffix("_release_cursor", true);

            var camShowHide = this.ApplyInfo.Target.AddComponent<CamLockSessionShowHide>();

            camShowHide.ShowRoot = MelGameObjectHelper.FindInRoot(
                this.ApplyInfo.Target.transform,
                this.GetStringWithSuffix("_show_root_object"),
                modelPostProcessInfo.ImportHierarchyLookup);

            camShowHide.HideRoot = MelGameObjectHelper.FindInRoot(
                this.ApplyInfo.Target.transform,
                this.GetStringWithSuffix("_hide_root_object"),
                modelPostProcessInfo.ImportHierarchyLookup);

            //  If this object was a camera in blender, there will be a Camera component attached
            //   to the target object at this stage in the import process.
            camLockSession.TargetCamera = MelGameObjectHelper.AddIfNotPresent<Camera>(this.ApplyInfo.Target);

            this.ApplyToEnableable(camLockSession);
        }
    }
}