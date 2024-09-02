using UnityEngine;
using System.Threading.Tasks;

namespace DuksGames.Argon.Utils
{

    public class MouseDownResult
    {
        public bool IsMouseHold;

        public Task<float> MouseUp;

        public static async Task<MouseDownResult> CreateFrom(Task<float> MouseUp, Task ClickTimeElapsed)
        {
            var result = await Task.WhenAny(MouseUp, ClickTimeElapsed);
            return new MouseDownResult
            {
                IsMouseHold = result == ClickTimeElapsed,
                MouseUp = MouseUp
            };
        }
    }

    public class MouseTask : Singleton<MouseTask>
    {
        public int MouseButton = 0;

        TaskCompletionSource<float> _mousePressCompletion;
        float _downTimestamp;


        bool IsMouseDownTaskRunning
        {
            get { return this._mousePressCompletion != null && this._mousePressCompletion.Task.Status <= TaskStatus.Running; }
        }

        void TryComplete()
        {
            if (!this.IsMouseDownTaskRunning)
            {
                return;
            }

            this._mousePressCompletion.TrySetResult(Time.time - this._downTimestamp);
        }

        public Task<float> NextMouseUp()
        {
            if (this._mousePressCompletion == null)
            {
                this._mousePressCompletion = new TaskCompletionSource<float>();
                this._downTimestamp = Time.time;
            }

            return this._mousePressCompletion.Task;
        }

        /// <summary>
        /// Returns a MouseDownResult as a Task. Use the returned object to check whether 
        ///  an already-in-progress mouse down is a mouse hold or a click. This method assumes that the
        ///   mouse is already down. 
        /// </summary>
        /// <returns></returns>
        public Task<MouseDownResult> NextMouseDownResult()
        {
            return MouseDownResult.CreateFrom(this.NextMouseUp(), AwaitableTimer.Instance.Wait(.08f));
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(this.MouseButton))
            {
                this.TryComplete();
                this._mousePressCompletion = null;
            }
        }
    }
}