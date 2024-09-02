// using UnityEngine;

// namespace DuksGames.LogicBlocks
// {
//     /// <summary>
//     /// Interpret a speed as on or off
//     /// </summary>
//     public class SpeedOnOffProvider :  AbstractOnOffStateProvider
//     {
//         // Our way of holding a reference to an interface that can survive Unity serialization.
//         //   Hopefully a better way exists.
//         public Component ISpeedProviderLink; 
//         ISpeedProvider SpeedProvider => (ISpeedProvider)this.ISpeedProviderLink;

//         public override bool IsOn()
//         {
//             return SpeedProvider.GetSpeed() > 0d;
//         }

//     }
// }