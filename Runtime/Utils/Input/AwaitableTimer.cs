using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections;

namespace DuksGames.Argon.Utils
{
    public class AwaitableTimer : Singleton<AwaitableTimer>
    {

        public Task Wait(float seconds, object result = null)
        {
            var completion = new TaskCompletionSource<object>();
            StartCoroutine(this.Wait(seconds, () =>
            {
                completion.SetResult(result);
            }));
            return completion.Task;
        }

        IEnumerator Wait(float seconds, Action completed)
        {
            yield return new WaitForSeconds(seconds);
            completed.Invoke();
        }
    }
}