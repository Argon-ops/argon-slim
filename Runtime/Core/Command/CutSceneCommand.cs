using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Event;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Shared;
using DuksGames.Argon.Config;
using System.Collections.Generic;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{

    public class CutSceneCommand : AbstractOvertimeCommand, INormalizedProgressProvider, IDDebugGetClipWrapper
    {
        public PlayableClipWrapper Playable;

        public Camera CutSceneCamera;

        public bool IsCancellable;

        void Start()
        {
            this.SetupWrapper();
            this.StartListening();
        }

        void OnDestroy()
        {
            this.Teardown();
            this.StopListening();
        }

        class PlayCommandCompletion
        {
            public TaskCompletionSource<CommandResult> Completion;
            public CommandInfo Info;
        }

        CursorDataListener OnEndedListener;
        PlayCommandCompletion PlayCompletion = new();

        protected bool IsNotPlaying
        {
            get
            {
                return this.PlayCompletion.Completion == null ||
                    this.PlayCompletion.Completion.Task.Status > TaskStatus.Running;
            }
        }


        protected virtual void HandlePlayEnded(LimitNotifyPlayable.PlayCursorData data)
        {
            // Debug.Log($"HANDLE PLAY ENDED null check: {this.PlayCompletion == null} | playCompl.Completion: {this.PlayCompletion?.Completion == null}".Blue());

            if (this.IsNotPlaying) 
            {
                return;
            }
            this.Playable.Stop();
            try
            {
                this.PlayCompletion.Completion.TrySetResult(new CommandResult
                {
                    CommandInfo = this.PlayCompletion.Info,
                    Type = CommandResultType.Completed
                });
            }
            catch (System.ObjectDisposedException)
            {
                Debug.LogWarning($"object disposed exception in HandlePlayEnded from {this.name}");
            }
            finally
            {
                SceneServices.Instance.CamSwap.SwapToMain();
            }
        }

        protected virtual void RestartPlay(CommandInfo commandInfo)
        {
            this.Playable.RestartPlay(true);
        }

        protected override async Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            SceneServices.Instance.CamSwap.SwapTo(this.CutSceneCamera);
            SceneServices.Instance.ThirdPersonControllerStateManager.SetState(ThirdPersonControllerState.InCamLock);
            var fakeUpdateLoop = new FakeUpdateLoop();
            SceneServices.Instance.PlayerUpdateStack.TakeOver(fakeUpdateLoop);

            // this.InterruptPreviousPlay();
            Debug.Log($"_EXECUTE: Completion null: {this.PlayCompletion.Completion == null}");
            this.PlayCompletion.Completion = new TaskCompletionSource<CommandResult>();
            // Debug.Log($"Completion null: {this.PlayCompletion.Completion == null}".Pink());

            this.PlayCompletion.Info = commandInfo;

            this.RestartPlay(commandInfo);

            var tasks = new List<Task<CommandResult>>
            {
                this.PlayCompletion.Completion.Task
            };

            if(this.IsCancellable)
            {
                var nextCancelKey = AwaitNextButtonTask.Await(this.gameObject, new string[] { "AltCancel" }, new CommandResult
                {
                    CommandInfo = commandInfo,
                    Type = CommandResultType.Interrupted
                });
                tasks.Add(nextCancelKey);
            }

            var result = await await Task.WhenAny(tasks); // this.PlayCompletion.Completion.Task;

            Debug.Log($"CUTSCENE: Got result: {result.Type}");
            SceneServices.Instance.CamSwap.SwapToMain();
            SceneServices.Instance.ThirdPersonControllerStateManager.SetState(ThirdPersonControllerState.InGame);
            SceneServices.Instance.PlayerUpdateStack.Release(fakeUpdateLoop);

            return result; // this.PlayCompletion.Completion.Task;
        }


        void StartListening()
        {
            if (this.OnEndedListener != null && this.Playable.HasOnEndedListener(this.OnEndedListener))
            {
                return;
            }
            this.OnEndedListener = this.Playable.AddOnEndedListener(data => this.HandlePlayEnded(data));
        }

        void StopListening()
        {
            if (this.OnEndedListener != null)
            {
                this.OnEndedListener.StopListening();
            }
        }

        void SetupWrapper()
        {
            if (this.Playable.IsBuilt)
            {
                return;
            }
            this.Playable.WakeUp();
        }

        void Teardown()
        {
            this.Playable.DestroyGraph();
        }

        public double GetNormalizedProgress()
        {
            return this.Playable.GetNormalizedProgress01();
        }

        public override double GetSpeed()
        {
            return this.Playable.GetSpeed();
        }

        public PlayableClipWrapper DGetClipWrapper()
        {
            return this.Playable;
        }


    }

}
