using UnityEngine;
using UnityEngine.UIElements;

namespace DuksGames.Argon.DemoScene
{
    public class DemoScenePauseMenu : MonoBehaviour
    {

        VisualElement root => this.GetComponent<UIDocument>().rootVisualElement;

        [SerializeField] string _pauseScreenName = "pause-screen";

        public bool IsOverlayEnabled()
        {
            var elem = this.root.Q<VisualElement>(this._pauseScreenName);
            return elem.style.display != DisplayStyle.None;
        }

        public void ShowHide(bool shouldShow)
        {
            var elem = this.root.Q<VisualElement>(this._pauseScreenName);
            elem.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
        }


    }
}