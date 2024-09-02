using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Utils
{
    public class AwaitNextButtonTask : MonoBehaviour
    {
        public string[] Names;

        public Task Await()
        {
            Assert.IsTrue(this.Names.Length > 0);
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(this.Poll(tcs));
            return tcs.Task;
        }

        IEnumerator Poll(TaskCompletionSource<bool> tcs)
        {
            bool shouldBreak = false;
            while (!shouldBreak)
            {
                foreach (var name in this.Names)
                {
                    if (Input.GetButtonDown(name))
                    {
                        shouldBreak = true;
                        break;
                    }
                }

                yield return null;
            }
            tcs.TrySetResult(true);
        }

        public static async Task<T> Await<T>(GameObject owner, string[] names, T result)
        {
            var awaiter = owner.AddComponent<AwaitNextButtonTask>();
            awaiter.Names = names;
            await awaiter.Await();
            GameObject.Destroy(awaiter);
            return result;
        }

    }
}