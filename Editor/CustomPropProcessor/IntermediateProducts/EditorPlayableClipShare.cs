using System.Collections.Generic;
using DuksGames.Argon.Animate;
using UnityEngine;

namespace DuksGames.Tools
{

    public class PlayableFabricationInfo
    {
        public PlayableClipIngredients PlayableClipIngredients;

        public Transform Owner;

        public int PlayableType;

        public bool AllowsInterrupts;

        public StarterWorkTicketData WorkTicketData;

        public ModelPostProcessInfo ModelPostProcessInfo;
    }

    public class EditorPlayableClipShare
    {

        public Dictionary<string, PlayableFabricationInfo> PlayableIngredients = new Dictionary<string, PlayableFabricationInfo>();

        public void Clear()
        {
            this.PlayableIngredients.Clear();
        }
    }
}