using DuksGames.Argon.Adapters;
using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class Spawner : AbstractThresholdInterpreter
    {
        public GameObject Prefab;

        protected override void SetOnOff(bool isEnabled)
        {
            if (!isEnabled) { return; }

            GameObject.Instantiate(this.Prefab, this.transform.position, this.transform.rotation);
        }

        protected override bool GetState()
        {
            return false;
        }
    }
}