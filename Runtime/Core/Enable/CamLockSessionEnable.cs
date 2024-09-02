using System.Threading.Tasks;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace DuksGames.Argon.Core
{
    public class CamLockSessionEnable : AbstractThresholdInterpreter
    {
        public Camera TargetCamera;
        public bool ShouldReleaseCursor;

        public UnityEvent<GameObject> OnInteractionBegin;
        public UnityEvent<PlayerInteractionResult> OnInteractionEnded;
        IPlayerInteractionTask _playerInteractionTask => this.GetComponent<IPlayerInteractionTask>();
        Task<PlayerInteractionResult> _currentSession;

        IUpdateLoop fakeUpdateLoop = new BlankUpdateLoop();

        bool IsRunning => this._currentSession != null;

        protected override bool GetState()
        {
            return this.IsRunning;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            Debug.Log($"CAM LOCK set on off");
            if (!isEnabled)
            {
                Debug.Log($"Cam lock: not enabled");
                // end an active session if there is one
                this._playerInteractionTask.CancelSession();
                return;
            }
            if (this.IsRunning)
            {
                Debug.Log($"Cam lock already running. won't run again");
                return;
            }

            this.RunSession();
        }

        async void RunSession()
        {
            Debug.Log($"Start cam lock session: {this.name} | interaction has a pInterTask: {this._playerInteractionTask != null} | OnInterBegin? {this.OnInteractionBegin != null}");
            this.SwapTo();
            await Task.Yield();
            this.OnInteractionBegin.Invoke(this.gameObject);

            SceneServices.Instance.HUDStateManager.RequestHUDState(HUDStateRequest.Hide);

            Assert.IsFalse(this._playerInteractionTask == null, $"_playerInteractionTask is null on {this.name}. Was expecting to find an IPlayerInteractionTask component");

            this._currentSession = this._playerInteractionTask.CompleteSession();
            this.OnInteractionEnded.Invoke(await this._currentSession);

            SceneServices.Instance.HUDStateManager.RequestHUDState(HUDStateRequest.Show);

            this._currentSession = null;
            this.SwapFrom();
        }

        void SwapTo()
        {
            // use SceneServices's UpdateStack to switch to an empty update loop
            //   That we release in SwapFrom. 

            // TODO: mouse caster's coroutine pattern is nice but
            //   it would be a lot better if the ReTwoPickSession ran on a callback from fakeUpdateLoop
            //    (which is no longer so fake; so rename it)
            //  ALSO: This isn't CamLockSessionEnable's job and its an over help on its part.
            //  So, let ReTwoPick Session take over the update stack with its own loop and 
            //    run mouse pick session with that (and re write mouse pick session a little)
            //  REASON BEING: When there is more than one non main loop loop on the UpdateStack
            //     there's a chance for a bug if: 
            //   loopA takes over
            //    then loopB takes over (B is top of the stack)
            //   loopA tries to release but the release fails because of UpdateStacks behaviour
            //     it only releases if the top (current) loop is asking to release
            //    loopA misses its chance to release and never asks at the 'correct' time
            //
            // We should prob. rewrite the mouse caster and actually use the loop instead of having a fake update loop
            //   but should we also tolerate out-of-order release requests?
            //     

            // SceneServices.Instance.PlayerUpdateStack.TakeOver(this.fakeUpdateLoop);
            SceneServices.Instance.CamSwap.SwapTo(this.TargetCamera);
            if (this.ShouldReleaseCursor)
                SceneServices.Instance.CursorLocker.FreeCursor();
        }

        void SwapFrom()
        {
            SceneServices.Instance.CamSwap.SwapToMain();
            if (this.ShouldReleaseCursor)
                SceneServices.Instance.CursorLocker.LockCursor();
            // SceneServices.Instance.PlayerUpdateStack.Release(this.fakeUpdateLoop);
        }
    }
}