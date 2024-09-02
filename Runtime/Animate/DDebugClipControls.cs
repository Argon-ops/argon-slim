using UnityEngine;
using UnityEditor;

namespace DuksGames.Argon.Animate
{
    public interface IDDebugGetClipWrapper
    {
        PlayableClipWrapper DGetClipWrapper();
    }

    public class DDebugClipControls : MonoBehaviour
    {
        IDDebugGetClipWrapper clipWrapperSource;

        void Start()
        {
            this.clipWrapperSource = GetComponent<IDDebugGetClipWrapper>();
        }
        void Update()
        {
            // this.clipWrapperSource.DGetClipWrapper()._DBUG_PCWUpdate();
        }
    }
}