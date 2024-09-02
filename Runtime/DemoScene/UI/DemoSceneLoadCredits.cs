using UnityEngine;
using UnityEngine.UIElements;

namespace DuksGames.Argon.DemoScene
{
    public class DemoSceneLoadCredits : MonoBehaviour
    {

        [SerializeField] TextAsset credits;

        void Start() {
            var root = this.GetComponent<UIDocument>().rootVisualElement;
            var credits = root.Q<Label>("credits-text");

            credits.text = this.credits.text;
            GameObject.Destroy(this); 

        }

    }
}