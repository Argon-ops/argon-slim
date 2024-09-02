
namespace DuksGames.Argon.Adapters
{
    public interface IProximityClickManager
    {
        void Add(IChoosableClickBeacon clickBeacon);
        void Remove(IChoosableClickBeacon clickBeacon);
    }

    public interface ICurrentBeaconProvider
    {
        IChoosableClickBeacon GetCurrentBeacon();
    }
}