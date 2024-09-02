using UnityEngine;
using UnityEditor;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Core
{
    public class ConstantValueModifier : AbstractCommandInfoModifier
    {
        public float ConstantValue;

        protected override void _Modify(ref CommandInfo commandInfo)
        {
            commandInfo.Signal = this.ConstantValue;
        }
    }
}