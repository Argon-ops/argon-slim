using UnityEngine;
using DuksGames.Argon.Adapters;
using System.Collections;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Core
{
    public class InitialEnableStateSetter : MonoBehaviour
    {
        public bool InitialState;

        public Component ISignalHandlerLink;
        ISignalHandler Enableable => (ISignalHandler)this.ISignalHandlerLink;

        void Start()
        {
            StartCoroutine(this._AfterAFrame());
        }

        IEnumerator _AfterAFrame()
        {
            // It's preferable to wait for the end of the first frame.
            //   Otherwise, in the case where this causes a disable on an object,
            //    After disabling, other components on this object might not get their Start methods called.
            //  We can't think of any ISignalHandler that will care about this slight delay.
            //   Making these initialization calls configurable in terms of execution order seems like over-kill
            yield return new WaitForEndOfFrame();

            this.Enableable.HandleISignal(this.InitialState ? 1d : 0d);
            GameObject.Destroy(this);
        }
    }
}