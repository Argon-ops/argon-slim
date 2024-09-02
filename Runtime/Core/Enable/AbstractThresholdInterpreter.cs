using System;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.PropertyDrawers;
using UnityEngine;

namespace DuksGames.Argon.Core
{

    public interface IThresholdFilter
    {
        bool Filter(double val);
    }

    [System.Serializable]
    public class ThresholdSettings
    {
        public bool Clamp01;
        public bool Invert;
        public double Threshold = .5d;
    }

    // REMINDER: kinds of on / off:
    //  A: express some state e.g. particle system playing / stopping
    //  B:  be responsive or unresponsive: should i respond to or ignore signals asking me to express some state
    //  In Threshold Interpreters, sleeping and waking means becoming responsive or unresponsive in the second sense of on / off.

    public abstract class AbstractThresholdInterpreter : MonoBehaviour, ISignalHandler, ISleep
    {
        public ThresholdSettings thresholdSettings = new();
        public bool IsSelfToggling;

        [TypeEnforce(typeof(IThresholdFilter))]
        public Component IThresholdFilterLink;
        IThresholdFilter _filter => (IThresholdFilter)this.IThresholdFilterLink;
        bool _isSleeping;

        bool Filter(double signal)
        {
            if (this.IThresholdFilterLink)
            {
                return this._filter.Filter(signal);
            }
            if (this.IsSelfToggling)
            {
                return !this.GetState();
            }
            signal = this.thresholdSettings.Clamp01 ? Math.Clamp(signal, 0d, 1d) : signal;
            signal = this.thresholdSettings.Invert ? 1d - signal : signal;
            return signal > this.thresholdSettings.Threshold;
        }

        public void HandleISignal(double signal)
        {
            if (this._isSleeping)
            {
                return;
            }
            this.SetOnOff(this.Filter(signal));
        }

        public void SetIsAwake(bool isAwake)
        {
            this._isSleeping = !isAwake;
        }

        public bool IsOn => GetState();

        protected abstract void SetOnOff(bool isEnabled);

        protected abstract bool GetState();

    }
}