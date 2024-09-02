using System.Collections.Generic;

namespace DuksGames.Tools
{
    public class TagParseKeySet : AbstractCustomPropKeySet<TagParser>
    {
        public override string TargetKey => "mel_tag";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.TargetKey;
        }

    }

    public class TagParser : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        public void Apply()
        {
            string tag = this.Config.getValue(this.KeySet.TargetKey);

            if (tag == null || tag.Length == 0)
            {
                return;
            }

            var matchingTag = MelGameObjectHelper.AddTag(tag);

            // error msg if tag doesn't exist in 'tags' array
            ApplyInfo.Target.tag = matchingTag;
        }
    }
}