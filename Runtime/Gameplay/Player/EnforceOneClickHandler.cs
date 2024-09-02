using UnityEngine;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Config;
using System;
using System.Collections;

namespace DuksGames.Argon.Gameplay
{

    /// <summary>
    /// Runs simple highlighting: interactables will highlight when 
    ///   the camera's ray intersects them. 
    /// Attach this component to 
    ///   your player's game object if you want simple highlighting. For
    ///   proximity highlighting, use ProximityClickProbe instead.
    /// </summary>
    public class EnforceOneClickHandler : MonoBehaviour
    {
        // object _current;

        

        // // TODO: purge this class. it is solving a problem the doesn't exist
        // public bool CanHandle(object candidate)
        // {
        //     var Dcur = ((Component)this._current);
        //     Debug.Log($"current is; '{Dcur?.name}'");

        //     if(this._current == null && candidate != null)
        //     {
        //         this._current = candidate;
        //         StartCoroutine(this.WaitAndNullCurrent());
        //         return true;
        //     }
        //     return false;
        // }

        // IEnumerator WaitAndNullCurrent()
        // {
        //     yield return new WaitForEndOfFrame();
        //     this._current = null;
        // }
    }
}