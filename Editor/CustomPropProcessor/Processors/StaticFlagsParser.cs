using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class StaticFlagsKeySet : AbstractCustomPropKeySet<StaticFlagsParser>
    {
        public override string TargetKey => "mel_static_flags";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.TargetKey;
        }

    }

    public class StaticFlagsParser : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            GameObjectUtility.SetStaticEditorFlags(
                ApplyInfo.Target,
                (StaticEditorFlags)Config.getInt(this.KeySet.TargetKey));
        }
    }
}