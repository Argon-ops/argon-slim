using UnityEngine;
using DuksGames.Argon.Adapters;
using System;
using System.Collections;

namespace DuksGames.Argon.Gameplay
{

    /// <summary>
    /// A stand-in for an actual inventory manager.
    /// </summary>
    public class RETwoFakeInventoryManager : MonoBehaviour, IInventoryManager
    {
        public bool Acquire(GameObject item)
        {
            StartCoroutine(FakeDestroy(item));
            return true;
        }

        public int Contains(string itemName)
        {
            return 0;
        }

        public GameObject Provide(string itemName)
        {
            return null;
        }

        public bool Remove(string itemName)
        {
            throw new System.NotImplementedException("This fake inventory class doesn't really manange an inventory");
        }

        IEnumerator FakeDestroy(GameObject item)
        {
            item.SetActive(false);
            yield return new WaitForSeconds(2f);
            if (item)
            {
                item.SetActive(true);
            }
        }
    }
}