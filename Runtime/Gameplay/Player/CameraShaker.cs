using UnityEngine;
using DuksGames.Argon.Adapters;
using System.Threading.Tasks;
using System.Collections;

namespace DuksGames.Argon.Gameplay
{
    public class CameraShaker : MonoBehaviour, ICameraShake
    {

        TaskCompletionSource<bool> _tcs;

        void CancelRunning()
        {
            if (this._tcs == null)
            {
                return;
            }
            this._tcs.TrySetResult(true);
            this._tcs = null;
        }

        public void StopShaking()
        {
            this.CancelRunning();
        }

        public Task Shake(CameraShakeConfig shakeConfig)
        {
            this.CancelRunning();

            this._tcs = new TaskCompletionSource<bool>(); // Pointless bool type non-generic TCS isn't available in Unity's dot-net
            StartCoroutine(this._Shake(shakeConfig, () =>
            {
                if (this._tcs == null) { return; }
                this._tcs.TrySetResult(true);
                this._tcs = null;
            }));

            return _tcs.Task;
        }

        IEnumerator _Shake(CameraShakeConfig shakeConfig, System.Action onComplete)
        {
            var cam = Camera.main;
            var baseLocalPosition = cam.transform.localPosition;
            var startTime = Time.time;

            // Shake for at least one frame even if ShakeDuration is very short
            do
            {
                if (this._tcs == null || this._tcs.Task.Status >= TaskStatus.Running) { break; }
                cam.transform.localPosition = baseLocalPosition + Random.insideUnitSphere * shakeConfig.DisplacementDistance;
                yield return new WaitForFixedUpdate();
            } while (startTime + shakeConfig.ShakeDuration > Time.time);

            cam.transform.localPosition = baseLocalPosition;
            onComplete.Invoke();
        }
    }
}

// NOTE: understand the dangers of tcs: https://devblogs.microsoft.com/premier-developer/the-danger-of-taskcompletionsourcet-class/