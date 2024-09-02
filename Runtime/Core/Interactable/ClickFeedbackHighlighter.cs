// using System;
// using System.Collections;
// using DuksGames.Argon.Interaction;
// using UnityEngine;

// namespace DuksGames.Argon.Core
// {
//     public class ClickFeedbackHighlighter : MonoBehaviour, IClickFeedback
//     {

//         AbstractHighlighter _highlighter;

//         void Start() {
//             this._highlighter = this.GetComponent<AbstractHighlighter>();
//         }

//         public void GiveFeedback()
//         {
//             StartCoroutine(FlashHighlighter());
//         }

//         IEnumerator FlashHighlighter()
//         {
//             for (int i = 0; i < 1; ++i)
//             {
//                 this._highlighter.Highlight(false);
//                 yield return new WaitForSeconds(.05f);
//                 this._highlighter.Highlight(true);
//                 yield return new WaitForSeconds(.2f);
//             }
//             this._highlighter.Highlight(false);
//         }
//     }
// }