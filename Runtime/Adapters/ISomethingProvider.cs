
namespace DuksGames.Argon.Adapters
{
    public interface ISomethingProvider<T>
    {
        T Provide();
    }

    public interface IResourceLibrarian<T> : ISomethingProvider<T>
    {
        void PutBack(T item);
    }

}