using System.Collections.Generic;
using DuksGames.Argon.Core;
using UnityEngine;

namespace DuksGames.Tools
{
    public class AudioEnableKeySet : EnableableKeySet<AudioEnableProcessor> //  AbstractCustomPropKeySet<AudioEnableProcessor>
    {
        public override string TargetKey => "mel_audio_enable";

        public override IEnumerable<string> GetAdditionalKeys()
        {
            yield return this.AppendSuffix("_clip_name");
            yield return this.AppendSuffix("_loop");
            yield return this.AppendSuffix("_on_signal_always_restarts");
        }
    }

    public class AudioEnableProcessor : EnableablePropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            var audioEnable = this.ApplyInfo.Target.AddComponent<AudioEnable>();

            audioEnable.looping = this.GetBoolWithSuffix("_loop", true);
            var clipName = this.GetStringWithSuffix("_clip_name");
            audioEnable.clip = MelGameObjectHelper.FindInProjectOrWarn<AudioClip>(clipName, "AudioClip", $"No audio clip on {this.ApplyInfo.Target.name}");
            audioEnable.onSignalAlwaysRestarts = this.GetBoolWithSuffix("_on_signal_always_restarts", true);

            this.ApplyToEnableable(audioEnable);
        }
    }
}