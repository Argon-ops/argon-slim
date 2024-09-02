// using UnityEngine;
// using UnityEditor;
// using DuksGames.Argon.Adapters;
// using DuksGames.Argon.Config;

// namespace DuksGames.Argon.Gameplay
// {
//     public class ProximityClickBeacon : MonoBehaviour, ISluggishProximityCallback
//     {
//         ProximityClickManager pcm;

//         void Start() {
//             this.pcm = SceneServices.Instance.PlayerProvider.GetPlayer().GetComponent<ProximityClickManager>();
//         }

//         public void HandleSluggishProximityUpdate(float distanceSquared, float radiusSquared, GameObject proximityOwner)
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }