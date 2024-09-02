using DuksGames.Argon.Adapters;
using UnityEngine;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Core
{

    public class InvisibleHighlighter : AbstractHighlighter
    {
        protected override Vector3 GetHighlightPosition()
        {
            return this.gameObject.transform.position;
        }

        protected override void SetHighlightState(EClickBeaconState state)
        {
        }

    }
}