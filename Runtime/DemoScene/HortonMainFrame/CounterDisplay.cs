using UnityEngine;
using TMPro;

namespace DuksGames.Argon.DemoScene
{
    public class CounterDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshPro Text;
        int _count;

        public void Increment()
        {
            this._count++;
            this.UpdateDisplay();
        }

        public void Decrement()
        {
            this._count--;
            this.UpdateDisplay();
        }

        void UpdateDisplay()
        {
            this.Text.text = $"{this._count}";
        }
    }
}