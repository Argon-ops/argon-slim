using DuksGames.Argon.Utils;
using DuksGames.Argon.Interaction;
using UnityEngine;
using DuksGames.Argon.Adapters;
using System;
using System.Collections;
using DuksGames.Argon.Shared;

namespace DuksGames.Argon.Gameplay
{
    public class HighlightManager : MonoBehaviour, IOneAtATimeHighlightManager
    {
        IInteractionHighlight _current; // => this.Transition.Current;

        // class HLTransition
        // {
        //     public IInteractionHighlight Current;
        //     public IInteractionHighlight Next;

        //     public bool IsTransitioning;
        // }

        // HLTransition Transition = new();

        public GameObject GetCurrent()
        {
            if(this._current == null)
            {
                return null;
            }
            return ((MonoBehaviour)this._current).gameObject;
        }

        // public void NextHighlight(IInteractionHighlight next)
        // {
        //     if (this._current == null)
        //     {
        //         this.Transition.Current = next;
        //         this._current?.Highlight(true);
        //         return;
        //     }

        //     if (this._current == next)
        //     {
        //         if(this.Transition.IsTransitioning)
        //         {
        //             this._shouldExitEarly = true;
        //         }
        //         return;
        //     }
        //     this.StartCoroutine(this.FadeCurrent(next));
        // }

       

        public void NextHighlight(IInteractionHighlight next)
        {
            if(this._current.IsNullOrDestroyed()) 
            {
                this._current = null;
            }
            if (this._current == next)
            {
                return;
            }

            // Debug.Log($"is current null: {this._current == null} | ref eq null: {object.ReferenceEquals(this._current, null)}. " + 
            //         $"Lord of Duct is NorDestroyed: {HighlightManager.IsNullOrDestroyed(this._current)} ");
            if (this._current != null)
            {
                if(((Component)this._current).gameObject != null)
                    this._current.Highlight(false);
                this._current = null;
            }
            if (next == null)
            {
                return;
            }
            this._current = next;
            this._current.Highlight(true);
        }

        // bool _shouldExitEarly;
        // [SerializeField] float _fadeTime = 1f;

        // IEnumerator FadeCurrent(IInteractionHighlight next)
        // {
        //     this._shouldExitEarly = true;
        //     yield return new WaitForEndOfFrame();
        //     this._shouldExitEarly = false;
        //     this.Transition.IsTransitioning = true;

        //     var start = Time.fixedTime;
        //     // while (start + this._fadeTime < Time.fixedTime)
        //     for(int i=0; i < 200; ++i)
        //     {
        //         if (this._shouldExitEarly)
        //         {
        //             break;
        //         }
        //         yield return new WaitForEndOfFrame();
        //     }

        //     this.Transition.IsTransitioning = false;

        //     if(!this._shouldExitEarly)
        //     {
        //         Debug.Log($"set to next: not null {next != null}");
        //         this._current.Highlight(false);
        //         this.Transition.Current = next;
        //         this._current?.Highlight(true);
        //     }
            
        // }
    }
}