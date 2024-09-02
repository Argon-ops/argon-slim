using System;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Animate;
using DuksGames.Argon.Event;
using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.DemoScene
{
    public struct NumericInputInfo
    {
        public string ButtonValue;
        public System.Action Cancel;
    }
    public class NumericInputController : MonoBehaviour
    {


        [System.Serializable]
        public class PButton
        {
            // NumericInputController_Inspector relies on these properties
            //   being named this way. Please never change these names or types.
            public PlayableClipWrapper Press;
            public Renderer Renderer;
            public string value;

            public Material DefaultMaterial { get; internal set; }
        }

        [SerializeField, Tooltip("Order must reflect layout. Row-major")] PButton[] _buttons;
        [SerializeField, Tooltip("How many columns wide is the button layout")] int _columns = 3;
        [SerializeField, Tooltip("How many rows of buttons")] int _rows = 4;

        [SerializeField] bool _invertVertical = true;

        [SerializeField] GameObject _pickSessionProviderLink;
        [SerializeField] Material _highlightMaterial;

        [SerializeField] AudioSource _cursorMoveAudio;

        Vector2Int _dimensions => new Vector2Int(this._columns, this._rows);
        Vector2Int _cursor;

        int _index => this._cursor.y * this._columns + this._cursor.x;

        bool _isCursorFocused
        {
            get
            {
                return this._cursor.x >= 0 && this._cursor.y >= 0;
            }
            set
            {
                if (value)
                {
                    this._cursor = Vector2Int.Max(Vector2Int.zero, this._cursor);
                    return;
                }
                this._cursor.y = -1;
            }
        }

        public UnityEvent<NumericInputInfo> OnInput;
        private Canceller _canceller;

        void InitButtons()
        {
            foreach (var b in this._buttons)
            {
                b.DefaultMaterial = b.Renderer.material;
            }
            foreach (var b in this._buttons)
            {
                b.Press.WakeUp();
            }
        }

        void TearDownButtons()
        {
            foreach (var b in this._buttons)
            {
                b.Press.DestroyGraph();
            }
        }

        void Start()
        {
            this.InitButtons();
            var pickSessionProvider = this._pickSessionProviderLink.GetComponent<IPickSessionInitInfoProvider>();
            pickSessionProvider.GetPickSessionInitInfo().AddListener(this.OnSessionInit);
        }

        void OnDestroy()
        {
            this.TearDownButtons();
        }

        private void OnSessionInit(PickSessionInitInfo initInfo)
        {
            this._isCursorFocused = false;
            initInfo.OnSessionUpdate.AddListener(this.OnSessionUpdate);
            this._canceller = initInfo.Canceller;
            // TODO: use the canceller >> keep a reference to it. (if null no cancel)
            //   TODO: provide NumerKeyLok with a mehanism for kanselling
        }


        private void OnSessionUpdate(RaycastHit hit, bool isHit)
        {
            if (!Input.anyKey)
            {
                return;
            }

            var nudge = new Vector2Int();
            if (Input.GetKeyDown(KeyCode.W))
            {
                nudge.y += 1 * (this._invertVertical ? -1 : 1);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                nudge.y += -1 * (this._invertVertical ? -1 : 1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                nudge.x += -1;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                nudge.x += 1;
            }

            this._isCursorFocused = true;
            this._cursor += nudge;
            this._cursor = Vector2Int.Min(this._dimensions - Vector2Int.one, Vector2Int.Max(Vector2Int.zero, this._cursor));
            this.UpdateButtons();

            if (Input.GetKeyDown(KeyCode.Space) || nudge.x != 0 || nudge.y != 0)
            {
                this._cursorMoveAudio.Play();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this._isCursorFocused)
                {
                    var button = this.GetCurrentButton();
                    this.OnInput.Invoke(new NumericInputInfo
                    {
                        ButtonValue = button.value,
                        Cancel = () =>
                        {
                            // TODO: cancel func. invoke our own canceler and clean up
                            this.TearDown();
                        }
                    });
                    button.Press.RestartPlay();
                }
            }
        }

        private void TearDown()
        {
            foreach (var b in this._buttons)
            {
                this.UpdateButton(b, false);
            }
            this._canceller?.Cancel();
        }

        PButton GetCurrentButton()
        {
            return this._buttons[this._index];
        }

        void UpdateButtons()
        {
            foreach (var b in this._buttons)
            {
                this.UpdateButton(b, false);
            }

            if (!this._isCursorFocused) { return; }

            this.UpdateButton(this.GetCurrentButton(), true);
        }

        void UpdateButton(PButton pButton, bool isHighlighted)
        {
            pButton.Renderer.material = isHighlighted ? this._highlightMaterial : pButton.DefaultMaterial;
        }
    }
}