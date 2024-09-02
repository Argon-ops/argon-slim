using UnityEngine;
using UnityEditor;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{
    [System.Serializable]
    public class ToggleModifier : AbstractCommandInfoModifier
    {
        public Component IOnOffStateProviderLink;
        IOnOffStateProvider OnOffStateProvider => (IOnOffStateProvider)this.IOnOffStateProviderLink;

        protected override void _Modify(ref CommandInfo commandInfo)
        {
            commandInfo.Signal = this.OnOffStateProvider.IsOn() ? 0f : 1f;
        }
    }
}