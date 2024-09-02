
namespace DuksGames.Argon.Adapters
{
    public interface ISleep
    {
        void SetIsAwake(bool isAwake);
    }

    public interface IIsAwakeProvider
    {
        bool GetIsAwake();
    }
}