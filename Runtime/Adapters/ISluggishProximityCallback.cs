using UnityEngine;

namespace DuksGames.Argon.Adapters
{
    public interface ISluggishProximityCallback
    {
        void HandleSluggishProximityUpdate(float distanceSquared, float radiusSquared, GameObject proximityOwner);
    }

    public interface ISluggishProximityPosition
    {
        Vector3 GetWorldPosition();
    }
}