namespace DuksGames.Argon.Adapters
{
    public interface ICursorLocker
    {
        void FreeCursor();
        void LockCursor();
    }
}