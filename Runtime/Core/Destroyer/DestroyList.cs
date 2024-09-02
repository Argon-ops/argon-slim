using System.Collections.Generic;
using DuksGames.Argon.Utils;
using UnityEngine;

namespace DuksGames.Argon.Core
{

    public class DestroyList : MonoBehaviour
    {
        [SerializeField] List<Component> _components = new();
        [SerializeField] List<GameObject> _gameObjects = new();

        public void Add(Component c)
        {
            if (c == null)
            {
                return;
            }

            this._components.Add(c);
        }

        public void AddGameObject(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            this._gameObjects.Add(go);
        }

        public void DestroyMembers()
        {

            foreach (var c in this._components)
            {
                if (c is IWillGetDestroyedByDestroyList)
                {
                    ((IWillGetDestroyedByDestroyList)c).WillGetDestroyedByDestroyList();
                }
            }
            foreach (var c in this._components)
            {
                GameObject.Destroy(c);
            }

            GameObject.Destroy(this);

            foreach (var go in this._gameObjects)
            {
                GameObject.Destroy(go);
            }
        }

    }
}