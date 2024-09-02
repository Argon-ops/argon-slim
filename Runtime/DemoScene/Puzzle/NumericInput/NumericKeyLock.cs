using UnityEngine;
using TMPro;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.DemoScene
{
    public class NumericKeyLock : MonoBehaviour
    {
        [SerializeField] TextMeshPro Display;
        [SerializeField] string _unlockCode = "234";


        [SerializeField] Component OnCorrectLink;
        IExecuteAsync OnCorrect => this.OnCorrectLink.GetComponent<IExecuteAsync>();
        [SerializeField] Component OnIncorrectLink;
        IExecuteAsync OnIncorrect => this.OnIncorrectLink.GetComponent<IExecuteAsync>();

        string _input;
        bool _isInputLocked;

        public void OnKeyInput(NumericInputInfo numericInputInfo)
        {
            var keyValue = numericInputInfo.ButtonValue;

            if (this._isInputLocked)
            {
                return;
            }
            if (!int.TryParse(keyValue, out int res))
            {
                this.Reset();
                return;
            }
            this._input += keyValue;
            this.UpdateDisplay();
            if (this._input == this._unlockCode)
            {
                this.Display.text = "___";
                this.Unlock(numericInputInfo.Cancel);
                return;
            }
            if (this._input.Length >= this._unlockCode.Length)
            {
                this.Incorrect();
            }

        }

        void UpdateDisplay()
        {
            this.Display.text = this._input;
        }

        async void Incorrect()
        {
            this._isInputLocked = true;
            await this.OnIncorrect.ExecuteAsync(new CommandInfo
            {
                Initiator = this.gameObject,
                Signal = 1f
            });
            this._isInputLocked = false;
            this.Reset();
        }

        void Reset()
        {
            this._input = "";
            this.UpdateDisplay();
        }

        async void Unlock(System.Action canceller)
        {
            // play unlock
            await this.OnCorrect.ExecuteAsync(new CommandInfo
            {
                Initiator = this.gameObject,
                Signal = 1f
            });

            canceller.Invoke();
            this.Reset();
        }
    }
}