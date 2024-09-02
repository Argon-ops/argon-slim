using UnityEngine;
using UnityEngine.UIElements;
using DuksGames.Argon.Adapters;
using System.Threading.Tasks;
using System.Collections;

namespace DuksGames.Argon.Gameplay
{
    public class HeadlineFeedback : MonoBehaviour, IHeadlineDisplay
    {
        VisualElement root => this.GetComponent<UIDocument>().rootVisualElement;

        Label headlineLabel;

        void Awake()
        {
            this.headlineLabel = this.root.Q<Label>("Headline");
        }

        public void Hide()
        {
            this._Hide();
        }

        public Task<int> Display(HeadlineDisplayInfo headlineDisplayInfo)
        {
            this._Show(headlineDisplayInfo);
            var tsc = new TaskCompletionSource<int>();

            StartCoroutine(this.Wait(headlineDisplayInfo.Seconds, tsc, () =>
            {
                this._Hide();
            }));
            return tsc.Task;
        }

        IEnumerator Wait(float seconds, TaskCompletionSource<int> tsc_, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            tsc_.TrySetResult(0); // return an int because we might want to support interrupting
            callback();
        }

        void _Show(HeadlineDisplayInfo headlineDisplayInfo)
        {
            this.headlineLabel.text = headlineDisplayInfo.Text;
            this.headlineLabel.style.display = DisplayStyle.Flex;
        }

        void _Hide()
        {
            this.headlineLabel.text = "";
            this.headlineLabel.style.display = DisplayStyle.None;
        }


    }
}