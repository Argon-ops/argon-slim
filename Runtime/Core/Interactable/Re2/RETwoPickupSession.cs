using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Utils;
using System.Threading.Tasks;
using UnityEngine.Events;
using DuksGames.Argon.Config;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Event;
using System;

namespace DuksGames.Argon.Core
{

    public enum PickSessionMode
    {
        MouseInput, KeyboardInput, MouseAndKeyboardInput
    }

    public class RETwoPickupSession : MonoBehaviour, IPlayerInteractionTask, IPickSessionInitInfoProvider
    {
        public UnityEvent<PickSessionInitInfo> OnSessionStart;
        UnityEvent<PickSessionInitInfo> IPickSessionInitInfoProvider.GetPickSessionInitInfo() { return this.OnSessionStart; }
        public UnityEvent<GameObject> OnItemPick;


        public bool ShouldCallClickHandlers;
        public bool TryAddingToInventory;
        public string[] CancelButtonsNames;

        Canceller externalCanceller;

        Task<PlayerInteractionResult> IPlayerInteractionTask.CompleteSession()
        {
            var tcs = new TaskCompletionSource<PlayerInteractionResult>();
            this.Run(tcs);
            return tcs.Task;
        }

        void IPlayerInteractionTask.CancelSession()
        {
            if (this.externalCanceller == null) { return; }
            this.externalCanceller.Cancel();
        }

        async void Run(TaskCompletionSource<PlayerInteractionResult> tcs)
        {
            this.externalCanceller = new();

            await Task.Yield(); // waiting a frame will allow any ButtonDown flags from the preceding frame to clear

            var mouseCaster = this.gameObject.AddComponent<MousePosRaycastProbe>();
            mouseCaster.SetLayerMask("CamLockPickable"); // TODO: document this

            var inventory = SceneServices.Instance.InventoryManager;

            SceneServices.Instance.ThirdPersonControllerStateManager.SetState(ThirdPersonControllerState.InCamLock);

            var OnMouseHit = new UnityEvent<RaycastHit>();
            var OnSessionUpdate = new UnityEvent<RaycastHit, bool>();
            var OnSessionEnd = new UnityEvent(); // CONSIDER: how does anyone access this??

            var canceller = new Canceller();
            this.OnSessionStart.Invoke(new PickSessionInitInfo
            {
                Canceller = canceller,
                OnSessionUpdate = OnSessionUpdate,
                OnMouseCastHit = OnMouseHit, 
                OnSessionEnded = OnSessionEnd
            });

            IClickInteractionHandler cached_handler;
            InteractionHandlerInfo cached_info;

            var updateLoop = new PickSessionUpdateLoop
            {
                Tick = () =>
                {
                    mouseCaster.Tick((hit, isHit) =>
                    {
                        OnSessionUpdate.Invoke(hit, isHit);

                        if (!isHit) 
                        { 
                            SceneServices.Instance.CamLockSessionOneAtATimeHighlightManager.NextHighlight(null);  
                            return; 
                        }

                        cached_handler = hit.transform.GetComponent<IClickInteractionHandler>();
                        cached_info = new InteractionHandlerInfo
                        {
                            Source = SceneServices.Instance.PlayerProvider.GetPlayer(),
                            InteractionTarget = (Component)cached_handler
                        };
                        if (cached_handler != null && cached_handler.CouldInteract(cached_info))
                        {
                            SceneServices.Instance.CamLockSessionOneAtATimeHighlightManager.NextHighlight(hit.transform?.GetComponent<IInteractionHighlight>());
                        }

                        OnMouseHit.Invoke(hit);

                        if (!Input.GetMouseButtonDown(0)) { return; }

                        if (this.TryAddingToInventory)
                        {
                            inventory.Acquire(hit.transform.gameObject);
                        }

                        if (this.ShouldCallClickHandlers)
                        {
                            // var Dhandler = (Component)handler;
                            // Debug.Log($"Got handler '{Dhandler?.name}' hit transform: {hit.transform.name}");
                            if(cached_handler != null && cached_handler.CouldInteract(cached_info))
                            {
                                cached_handler.Interact(cached_info);
                            }
                        }

                        this.OnItemPick.Invoke(hit.collider.gameObject);
                    },
                    SceneServices.Instance.CamSwap.GetCurrent());
                }
            };

            SceneServices.Instance.PlayerUpdateStack.TakeOver(updateLoop);

            var nextCancelButton = this.gameObject.AddComponent<AwaitNextButtonTask>();
            nextCancelButton.Names = new string[] { "AltCancel" };

            var nextRightClick = this.gameObject.AddComponent<AwaitNextMouseButtonTask>();
            nextRightClick.Buttons = new int[] { 1 };

            await Task.WhenAny(
                nextCancelButton.Await(),
                nextRightClick.Await(),
                canceller.GetTask(),
                this.externalCanceller.GetTask());


            SceneServices.Instance.PlayerUpdateStack.Release(updateLoop);

            // clean up highlighting
            SceneServices.Instance.CamLockSessionOneAtATimeHighlightManager.NextHighlight(null);

            SceneServices.Instance.ThirdPersonControllerStateManager.SetState(ThirdPersonControllerState.InGame);  

            // CONSIDER: what if they pause to a config screen during any of this...

            mouseCaster.Stop();
            
            GameObject.Destroy(nextCancelButton);
            GameObject.Destroy(nextRightClick);
            GameObject.Destroy(mouseCaster);

            OnSessionEnd.Invoke();

            OnMouseHit.RemoveAllListeners();
            OnSessionUpdate.RemoveAllListeners();
            OnSessionEnd.RemoveAllListeners();

            this.externalCanceller = null;

            tcs.TrySetResult(new PlayerInteractionResult
            {
                Completed = true,
                Source = this.gameObject,
            });


        }

        class PickSessionUpdateLoop : IUpdateLoop
        {
            public Action Tick;
            public void DoIUpdateLoop()
            {
                this.Tick();
            }
        }
    }
}

