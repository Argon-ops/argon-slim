using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using DuksGames.Argon.Interaction;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    public class OverlayFeedback : MonoBehaviour, IOverlayEnable
    {
        VisualElement root => this.GetComponent<UIDocument>().rootVisualElement;

        public bool IsOverlayEnabled(string overlayName)
        {
            var elem = this.root.Q<VisualElement>(overlayName);
            return elem.style.display != DisplayStyle.None;
        }

        public void ShowHide(bool shouldShow, string overlayName)
        {
            var elem = this.root.Q<VisualElement>(overlayName);
            Debug.Log($"OFee show elem: {elem.name}".Pink());
            elem.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}