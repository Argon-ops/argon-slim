using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class ObjectEnable : AbstractThresholdInterpreter
    {
        protected override bool GetState()
        {
            return this.gameObject.activeSelf;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            if(isEnabled)
                Debug.Log($"Ob Enable {this.name} Received {isEnabled}".Orange());
            else 
                Debug.Log($"Ob Enable {this.name} Received {isEnabled}".Blue());

            this.gameObject.SetActive(isEnabled);
        }

    }
}