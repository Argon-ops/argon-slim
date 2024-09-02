using UnityEngine;

namespace DuksGames.LogicBlocks 
{
    public class ThresholdOnOffProvider :  AbstractOnOffStateProvider
    {
        // Our way of holding a reference to an interface that can survive Unity serialization.
        //   Hopefully a better way exists.
        public Component IProgressLinkObject; 
        INormalizedProgressProvider ProgressProvider => (INormalizedProgressProvider)this.IProgressLinkObject;


        public override bool IsOn()
        {
            return ProgressProvider.GetNormalizedProgress() > .5d;
        }
    }
}