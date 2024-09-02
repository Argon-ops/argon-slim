using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace DuksGames.Argon.Event
{
    public class Canceller
    {
        TaskCompletionSource<bool> tcs = new();

        public Task GetTask()
        {
            return this.tcs.Task;
        }

        public void Cancel()
        {
            this.tcs.TrySetResult(false);
        }
    }

    public class PickSessionInitInfo
    {
        public Canceller Canceller;
        public UnityEvent<RaycastHit> OnMouseCastHit;
        public UnityEvent<RaycastHit, bool> OnSessionUpdate;
        public UnityEvent OnSessionEnded;
    }

    public interface IPickSessionInitInfoProvider
    {
        public UnityEvent<PickSessionInitInfo> GetPickSessionInitInfo();
    }
}