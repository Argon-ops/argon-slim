using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    public static class SetupSleepStateSetter
    {

        public static void Setup(MonoBehaviour sleeper, GameObject target, System.Func<string, int> getIntWithSuffix)
        {
            var onOff = (TurnOnOffDirectiveType)getIntWithSuffix("_set_initial_sleep_state");
            if (onOff == TurnOnOffDirectiveType.DoNothing)
            {
                return;
            }
            var initialSleepStateSetter = MelGameObjectHelper.AddIfNotPresent<InitialSleepStateSetter>(target);
            initialSleepStateSetter.Directives.Add(
                new SleepInstruction
                {
                    Component = sleeper,
                    TurnOnOff = onOff
                });
        }
    }
}