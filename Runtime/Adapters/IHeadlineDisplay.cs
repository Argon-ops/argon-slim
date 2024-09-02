using System.Threading.Tasks;
using UnityEngine;

namespace DuksGames.Argon.Adapters
{
    public class HeadlineDisplayInfo
    {
        public string Text;
        public float Seconds = 3f;
    }

    public interface IHeadlineDisplay
    {
        // Returns a Task<int> because this makes using TaskCompletionSource
        //  easier. There is no non-generic TaskCompletionSrouce
        Task<int> Display(HeadlineDisplayInfo headlineDisplayInfo);
        void Hide();
    }
}