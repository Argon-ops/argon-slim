using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{
    public class SceneObjectsReferencer : MonoBehaviour
    {
        public string[] Targets;

        [SerializeField] GameObject[] _Objects;

        public GameObject[] Objects => this._Objects;

        void Awake()
        {
            this._Objects = this.Targets.Select(name => GameObject.Find(name)).ToArray();

            #if UNITY_EDITOR
            for(int i = 0; i < this.Targets.Length; ++i)
            {
                Assert.IsFalse(Objects[i] == null, $"Reference object: '{this.Targets[i]}' wasn't found");
            }
            #endif
        }
    }
}