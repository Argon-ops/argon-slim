using UnityEngine;
using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Core
{
    public class ClickHoldInteractionHandler : ClickInteractionHandler
    {

        public bool HandleDiscreteClicksAlso;
        public float DiscreteClickFakeHoldTime;

        public override void Interact(InteractionHandlerInfo handlerInfo)
        {
            this.AwaitMouseDown(handlerInfo);
        }

        async void AwaitMouseDown(InteractionHandlerInfo handlerInfo)
        {
            // briefly await mouseDownResult. We need a little time to know whether this is a mouse-hold or just a click
            var mouseDownResult = await MouseTask.Instance.NextMouseDownResult();

            if (mouseDownResult.IsMouseHold)
            {
                foreach (var cmd in this.Commands)
                    cmd.Execute(this.CreateCommandInfo(handlerInfo, this.SignalSource.GetEnterSignal()));

                await mouseDownResult.MouseUp;

                foreach (var cmd in this.Commands)
                    cmd.Execute(this.CreateCommandInfo(handlerInfo, this.SignalSource.GetExitSignal()));

                this.OnInteracted.Invoke(handlerInfo);
                return;
            }

            //  it's a click. do we not respond at all?
            if (!this.HandleDiscreteClicksAlso) { return; }

            // we do respond to clicks: simulate a quick mouse hold
            foreach (var cmd in this.Commands)
                cmd.Execute(this.CreateCommandInfo(handlerInfo, this.SignalSource.GetEnterSignal()));

            await AwaitableTimer.Instance.Wait(this.DiscreteClickFakeHoldTime);

            foreach (var cmd in this.Commands)
                cmd.Execute(this.CreateCommandInfo(handlerInfo, this.SignalSource.GetExitSignal()));

            this.OnInteracted.Invoke(handlerInfo);
        }
    }
}