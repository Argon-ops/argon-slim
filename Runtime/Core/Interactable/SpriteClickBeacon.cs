using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{

    // TODO: separate class parallel to InterHighlighter : InteractionClickBeacon handles sleeping
    public class SpriteClickBeacon : MonoBehaviour, IBeaconDevice // IClickBeacon // , IClickBeaconStateProviderHMMM
    {
        public GameObject NearbySprite;
        public GameObject WillClickSprite;

        Renderer _nearby;
        Renderer _willClick;

        void Awake()
        {
            this._nearby = this.NearbySprite.GetComponent<Renderer>();
            this._willClick = this.WillClickSprite.GetComponent<Renderer>();
            Assert.IsFalse(this._nearby == null, "ss nearby null");
            Assert.IsFalse(this._willClick == null, "ss will click null");
        }

        void _SetState(EClickBeaconState state)
        {
            // TODO: why are there scenarios where this is called after our GO has been destroyed?
            Assert.IsFalse(this._nearby == null, $"ss nearby null on obj: '{this?.name}' ");
            Assert.IsFalse(this._willClick == null, $"ss will click null {this?.name}");
            switch (state)
            {
                case EClickBeaconState.Off:
                    this._nearby.enabled = false;
                    this._willClick.enabled = false;
                    break;
                case EClickBeaconState.Visible:
                    this._nearby.enabled = true;
                    this._willClick.enabled = false;
                    break;
                case EClickBeaconState.IsNextClick:
                    this._nearby.enabled = false;
                    this._willClick.enabled = true;
                    break;
            }
        }

        public void SetDeviceState(EClickBeaconState clickBeaconState)
        {
            this._SetState(clickBeaconState);
        }

    }
}