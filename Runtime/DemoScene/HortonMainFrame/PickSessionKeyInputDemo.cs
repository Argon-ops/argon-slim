using UnityEngine;
using DuksGames.Argon.Event;
using DuksGames.Argon.Config;

namespace DuksGames.Argon.DemoScene
{
    /// <summary>
    /// Demo class that subscribes to updates from a pick session. During its update callback,
    ///  it polls for any key down events. If there is a key down, it increments a CounterDisplay
    ///  and invokes an IExecute--and we're assuming that the IExecute plays a button press animation.
    /// </summary>
    public class PickSessionKeyInputDemo : MonoBehaviour
    {
        public CounterDisplay CounterDisplay;

        public GameObject ButtonPlayableLink;
        IExecute _buttonPlayable;

        void Start()
        {
            this._buttonPlayable = this.ButtonPlayableLink.GetComponent<IExecute>();
            var pickSessionInitInfoProvider = this.GetComponent<IPickSessionInitInfoProvider>();
            pickSessionInitInfoProvider.GetPickSessionInitInfo().AddListener(this.OnPickSessionInit);
        }

        private void OnPickSessionInit(PickSessionInitInfo sessionInitInfo)
        {
            sessionInitInfo.OnSessionUpdate.AddListener(this.OnPickSessionUpdate);
        }

        private void OnPickSessionUpdate(RaycastHit hit, bool isHit)
        {
            if (Input.anyKeyDown)
            {
                this.CounterDisplay.Increment();
                this._buttonPlayable.Execute(new CommandInfo
                {
                    Initiator = SceneServices.Instance.PlayerProvider.GetPlayer(),
                    Signal = 1f
                });
            }

        }
    }
}
