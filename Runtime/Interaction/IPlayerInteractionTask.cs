using System.Threading.Tasks;

namespace DuksGames.Argon.Interaction
{
    public interface IPlayerInteractionTask
    {
        Task<PlayerInteractionResult> CompleteSession();

        void CancelSession();
        
    }
    
}