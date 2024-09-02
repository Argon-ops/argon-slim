using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Utils
{
    public class AwaitNextMouseButtonTask : MonoBehaviour
    {
        public int[] Buttons;

        public Task Await()
        {
            Assert.IsTrue(this.Buttons.Length > 0);
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(this.Poll(tcs));
            return tcs.Task;
        }

        IEnumerator Poll(TaskCompletionSource<bool> tcs)
        {
            bool shouldBreak = false;
            while (!shouldBreak)
            {
                foreach (var name in this.Buttons)
                {
                    if (Input.GetMouseButtonDown(name))
                    {
                        shouldBreak = true;
                        break;
                    }
                }

                yield return null;
            }
            tcs.TrySetResult(true);
        }

    }
}