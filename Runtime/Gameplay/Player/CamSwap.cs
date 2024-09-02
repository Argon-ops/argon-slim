using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;
using System.Collections.Generic;
using System.Collections;

namespace DuksGames.Argon.Gameplay
{
    public class CamSwap : MonoBehaviour, ICamSwap, ICamSwapManager
    {
        Camera _cachedMain;
        Camera _current;

        List<Camera> managed = new();

        [SerializeField] bool _disableManagedOnStart;

        void Start()
        {

            this._cachedMain = Camera.main;
            
            StartCoroutine(this.LateStart());
            // TODO: consider what behaviour we need. Will we ever have to stack camera enables?
            //   And then will they have to un-stack/disable out of order sometimes?
            //    Can we just allow for out of order un-stacking and assume that it's harmless--no need to 
            //     police it?
            //   Seems like, it's a yes: we might want to stack the cam enables. (E.g. press a button, see a door open somewhere else)
            //  Other question: do we want a service that simply turns off all cameras except main?
        }

        IEnumerator LateStart()
        {
            yield return null;
            if(this._disableManagedOnStart)
            {
                this.DisableAllManaged();
            }
        }

        public Camera GetCurrent()
        {
            if (this._current != null)
                return this._current;
            return this._cachedMain;
        }

        public void SwapTo(Camera cam)
        {
            this.DisableAllManaged();
            
            if (this._current)
                this._current.enabled = false;

            this._cachedMain.enabled = false;
            this._current = cam;
            this._current.enabled = true;
        }

        public void SwapFrom(Camera cam)
        {
            // TODO: stack
            if(cam != this._current)
            {
                return;
            }
            this.SwapToMain();
        }

        public void SwapToMain()
        {
            this.DisableAllManaged();

            if (this._current)
            {
                this._current.enabled = false;
                this._current = null;
            }

            this._cachedMain.enabled = true;
        }

        void DisableAllManaged()
        {
            foreach(var cam in this.managed)
            {
                cam.enabled = false;
            }
        }

        public void AddManaged(Camera cam)
        {
            this.managed.Add(cam);
        }
    }
}