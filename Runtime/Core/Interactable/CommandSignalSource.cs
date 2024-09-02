using UnityEngine;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Core
{

    public interface ICommandSignalSource
    {
        double GetEnterSignal();
        double GetExitSignal();
    }

    public class CommandSignalSource : MonoBehaviour, ICommandSignalSource, ISimpleDestroyReceiver
    {
        public Component IEnterScalarLink;
        IScalarProvider EnterMechanism => (IScalarProvider)this.IEnterScalarLink;

        public Component IExitScalarLink;
        IScalarProvider ExitMechanism => (IScalarProvider)this.IExitScalarLink;

        public double EnterSignal = 1f;
        public double ExitSignal = 0f;

        public double GetEnterSignal()
        {
            if (this.IEnterScalarLink != null)
            { // awkward
                return this.EnterMechanism.GetIScalar();
            }
            return this.EnterSignal;
        }

        public double GetExitSignal()
        {
            if (this.IExitScalarLink != null) { return this.ExitMechanism.GetIScalar(); }
            return this.ExitSignal;
        }
    }
}