
namespace DuksGames.Argon.Adapters
{
    public interface ICustomDestroyMessageReceiver
    {
        void PreCustomDestroyMessage();
        void CustomDestroy();
    }

    public interface ISimpleDestroyReceiver
    {
    }
}