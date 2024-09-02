using UnityEngine;

namespace DuksGames.Argon.Adapters
{

    public enum EClickBeaconState
    {
        Off, Visible, IsNextClick
    }

    public interface IClickBeacon
    {
        void SetState(EClickBeaconState mode);
    }

    public interface IClickBeaconStateProvider
    {
        EClickBeaconState GetState();
    }

    public interface ILocatableBeacon
    {
        // Allow choosable beacons to define their positions. Because
        //  often their visual center and transform.position will be different
        Vector3 GetApparentPosition();

        // Transform GetBeaconTransform();
        // bool IsPointingToBeacon(RaycastHit hit);
    }

    public interface IChoosableClickBeacon
    {
        IClickBeacon GetClickBeacon();
        ILocatableBeacon GetLocatableBeacon();
        GameObject GetInteractionHandlerObject();

        bool HasForwardFaceVector(ref Vector3 forwardNormal);
    }
}