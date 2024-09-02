

namespace DuksGames.Argon.Adapters
{
    public class TickingUpdateLoop : IUpdateLoop
    {
        public System.Action Tick;
        public void DoIUpdateLoop()
        {
            this.Tick();
        } 
    }
}