using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using DuksGames.Argon.Interaction;

namespace DuksGames.Argon.Gameplay
{
    public class OverlayInitialSetup : MonoBehaviour
    {
        public UIDocument UIDocument;

        [Tooltip("Each element of the UI document will be disabled at start up")]
        public string[] DisableElements;

        void Start()
        {
            foreach(var elemName in DisableElements)
            {
                var elem = this.UIDocument.rootVisualElement.Q<VisualElement>(elemName);
                elem.style.display = DisplayStyle.None;
            }
        }
    }
}