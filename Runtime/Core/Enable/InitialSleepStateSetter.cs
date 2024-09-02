using UnityEngine;
using DuksGames.Argon.Adapters;
using System.Collections.Generic;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    [System.Serializable]
    public struct SleepInstruction
    {
        public Component Component;
        public TurnOnOffDirectiveType TurnOnOff;
    }

    public class InitialSleepStateSetter : MonoBehaviour
    {
        [SerializeField]
        public List<SleepInstruction> Directives = new List<SleepInstruction>();

        void Start()
        {
            foreach (var instruction in this.Directives)
            {
                if (instruction.TurnOnOff == TurnOnOffDirectiveType.DoNothing)
                {
                    continue;
                }
                ((ISleep)instruction.Component).SetIsAwake(instruction.TurnOnOff == TurnOnOffDirectiveType.TurnOn);
            }

            GameObject.Destroy(this);
        }
    }
}