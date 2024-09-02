using UnityEngine;
using System.Threading.Tasks;
using DuksGames.Argon.Event;
using DuksGames.Argon.Utils;
using System.Threading;
using DuksGames.Argon.Shared;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{

    // must match ble
    public enum OutroBehaviour
    {
        None, Constant, ThresholdCondition
    }


    [System.Serializable]
    public struct OutroSettings
    {
        public OutroBehaviour OutroBehaviour;
        public float Threshold;
        public float OutroDestinationValue;
        public float OutroDestinationValueB;
        public float OutroSpeedMultiplier;
    }

    public class SignalOverTimeCommand : SignalMessageCommand
    {

        AbstractFunction _function;
        FunctionBounds _bounds;
        //TaskQueue _taskQueue = new();


        public OverTimeFunctionIngredients FunctionIngredients;
        public OutroSettings OutroSettings;
        public bool RunIndefinitely;

        public bool PickupFromLastState = true; // TEST (true)
        // float _lastSignal;
        float _lastAccumulatedTime;

        void Awake()
        {
            this._function = FunctionFactory.CreateAndInit(this.FunctionIngredients);
            this._bounds = FunctionFactory.BoundsFrom(this.FunctionIngredients.Parameters);
        }

        public void Dreinit()
        {
            this._function = FunctionFactory.CreateAndInit(this.FunctionIngredients);
            this._bounds = FunctionFactory.BoundsFrom(this.FunctionIngredients.Parameters);
        }

        CancellationTokenSource _outroCanceller;
        Task<float> _current;

        CancellationTokenSource _jumpToOutroCanceller;

        bool CancelToOutro(CommandInfo commandInfo)
        {
            if (commandInfo.Signal > .5f)
            {
                return false;
            }
            if (this._current == null || this._current.Status > TaskStatus.Running)
            {
                return false;
            }

            this._jumpToOutroCanceller?.Cancel();
            return true;
        }

        async Task<float> Interrupt(CommandInfo commandInfo)
        {
            if (this._current == null || this._current.Status > TaskStatus.Running)
            {
                return commandInfo.Signal;
            }

            this.ForceCancel();
            return await this._current;
        }

        void ForceCancel()
        {
            this._jumpToOutroCanceller?.Cancel();
            this._outroCanceller?.Cancel();
        }

        public bool AllowInterrupts;

        protected override async Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            // CONSIDER: we are keeping a reference to the current broadcast job and using it to flag whether there's a 
            //   job in progress: there's probably a better way.
            if (commandInfo.Signal < .5f)
            {
                if (this.CancelToOutro(commandInfo)) { }

                return CommandResult.CompletedResult(commandInfo);
            }

            if (!this.AllowInterrupts && this._current != null)
            {
                return CommandResult.CompletedResult(commandInfo);
            }

            try
            {
                // interrupts allowed: interrupt, if needed, and await completion of the previous task
                var prevSignal = await this.Interrupt(commandInfo);

                Assert.IsTrue(this._current == null, "Current task should always be null right before we start a new task. We are using it as a semaphore");

                this._current = this.RunBroadcast(commandInfo);

                commandInfo.Signal = await this._current;

            }
            catch (System.Exception e)
            {
                throw e; // not expecting this
            }
            finally
            {
                this._current = null;
            }
            return CommandResult.CompletedResult(commandInfo);
        }

        async Task<float> RunBroadcast(CommandInfo commandInfo)
        {
            var result = 0f;
            try
            {
                // outro
                this._outroCanceller = new CancellationTokenSource();
                this._jumpToOutroCanceller = new CancellationTokenSource();
                var broadcastTask = this.Broadcast(commandInfo, this._jumpToOutroCanceller.Token);

                var resultSignal = await broadcastTask;

                var outroTask = this.Outro(commandInfo, resultSignal, this._outroCanceller.Token);
                result = await outroTask;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                this._outroCanceller.Dispose();
                this._outroCanceller = null;
                this._jumpToOutroCanceller.Dispose();
                this._jumpToOutroCanceller = null;
            }
            return result;
        }

        async Task<float> Broadcast(CommandInfo commandInfo, CancellationToken ct)
        {
            var broadcastIntervalSeconds = this.FunctionIngredients.Parameters.TickIntervalSeconds;
            var timeInterval = this.FunctionIngredients.Parameters.TotalRangeSeconds;
            int pingCount = (int)(timeInterval / broadcastIntervalSeconds);

            float accumulatedTime = this.PickupFromLastState ? this._lastAccumulatedTime : 0f;


            float signal = 0f;
            Debug.Log($"acc time: {accumulatedTime} signal: {signal} ");

            for (int i = 0; this.RunIndefinitely || i < pingCount; ++i)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                    // no point in using the cancellation token?
                    // ct.ThrowIfCancellationRequested();
                }

                signal = this._function.GetSignal(accumulatedTime);
                // Debug.Log($"acc time: {accumulatedTime} signal: {signal} ");

                this.SendSignal(signal);
                await Task.Delay(Mathf.FloorToInt(broadcastIntervalSeconds * 1000f));
                accumulatedTime += broadcastIntervalSeconds;
            }

            this._lastAccumulatedTime = accumulatedTime;
            return signal;
        }


        async Task<float> Outro(CommandInfo DcI, float startSignal, CancellationToken ct)
        {
            if (this.OutroSettings.OutroBehaviour == OutroBehaviour.None)
            {
                return startSignal;
            }
            float destination = this.GetOutroDestination(startSignal);
            float signal = startSignal;
            float totalSignalRange = this._bounds.GetSize();
            var timeRange = 1.3f / Mathf.Max(.00001f, this.OutroSettings.OutroSpeedMultiplier); // TODO: make non fake
            timeRange *= Mathf.Abs(destination - startSignal) / totalSignalRange;
            var broadcastIntervalSeconds = this.FunctionIngredients.Parameters.TickIntervalSeconds;
            int pingCount = (int)(timeRange / broadcastIntervalSeconds);
            float aTime = 0f;


            for (int i = 0; i < pingCount; ++i)
            {
                if (ct.IsCancellationRequested)
                {
                    break; // don't throw
                }
                signal = Mathf.Lerp(startSignal, destination, aTime / timeRange);

                this.SendSignal(signal);
                await Task.Delay(Mathf.FloorToInt(broadcastIntervalSeconds * 1000f));
                aTime += broadcastIntervalSeconds;
            }

            return signal;
        }

        float GetOutroDestination(float startSignal)
        {
            switch (this.OutroSettings.OutroBehaviour)
            {
                case OutroBehaviour.Constant:
                default:
                    return this.OutroSettings.OutroDestinationValue;
                case OutroBehaviour.ThresholdCondition:
                    return startSignal > this.OutroSettings.Threshold ?
                        this.OutroSettings.OutroDestinationValue :
                        this.OutroSettings.OutroDestinationValueB;
            }
        }

        void SendSignal(float signal)
        {
            foreach (var messageReceiver in this.MessageReceivers)
            {
                messageReceiver.HandleISignal(signal);
            }
        }

        void OnDestroy()
        {
            this.ForceCancel();
        }
    }
}