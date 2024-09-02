using UnityEngine;
using DuksGames.Argon.Utils;
using System.Linq;
using DuksGames.Argon.Adapters;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{
    public class ComponentEnable : MonoBehaviour, IOnOffHandler
    {
        public bool ApplyToSelf;
        public bool SearchChildren;

        public string TypeName;
        public string Namespace;

        System.Type _targetType;

        void Awake()
        {
            this._targetType = DuksGameObjectHelper.FindType(this.TypeName, this.Namespace);
            Assert.IsTrue(this._targetType != null, "Null target type in ComponentEnable");

            this.DcheckAreThereTargets();
        }

        void DcheckAreThereTargets()
        {
            var targets = (this.SearchChildren ? this.GetComponentsInChildren(this._targetType) : this.GetComponents(this._targetType)).OfType<Component>();
            if (targets.Count() == 0)
            {
                Debug.LogWarning($"Component Enable on {this.name} has no targets to enable");
            }
        }

        public void HandleIOnOff(bool isOn)
        {
            this.SetOnOff(isOn);
        }

        void SetOnOff(bool isOn)
        {

            foreach (var c in (this.SearchChildren ? this.GetComponentsInChildren(this._targetType) : this.GetComponents(this._targetType)).OfType<Component>())
            {
                if (!this.ApplyToSelf && c == this)
                {
                    continue;
                }

                if (c is MonoBehaviour behaviour)
                {
                    behaviour.enabled = isOn;
                }
                else if (c is Renderer renderer1)
                {
                    renderer1.enabled = isOn;
                }
            }
        }
    }
}