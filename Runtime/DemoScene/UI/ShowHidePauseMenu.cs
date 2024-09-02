using DuksGames.Argon.Config;
using UnityEngine;

namespace DuksGames.Argon.DemoScene
{
    public class ShowHidePauseMenu : MonoBehaviour
    {
        [SerializeField] DemoScenePauseMenu _demoScenePauseMenu;

        void Start() {
            this._demoScenePauseMenu.ShowHide(false);
        }

        void Update() {
            if (Input.GetButtonDown("Cancel")) {
                var isVis = this._demoScenePauseMenu.IsOverlayEnabled();
                this._demoScenePauseMenu.ShowHide(!isVis);
            }
            
        }
    }
}