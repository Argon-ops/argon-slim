using System.Collections;
using DuksGames.Argon.Adapters;
using UnityEngine;

namespace DuksGames.Argon.Core
{
    public class ContinuousSpawner : AbstractThresholdInterpreter //  MonoBehaviour, ISignalHandler
    {
        public GameObject Prefab;
        public float Interval;

        bool _isOn;

        protected override bool GetState()
        {
            return this._isOn;
        }

        protected override void SetOnOff(bool isEnabled)
        {
            if (!this._isOn)
            {
                StartCoroutine(Run());
                return;
            }
            this._isOn = false;
        }

        IEnumerator Run()
        {
            if (!this._isOn)
            {
                this._isOn = true;
                while (this._isOn)
                {
                    GameObject.Instantiate(this.Prefab, this.transform.position, this.transform.rotation);
                    yield return new WaitForSeconds(this.Interval);
                }
            }
        }
    }
}