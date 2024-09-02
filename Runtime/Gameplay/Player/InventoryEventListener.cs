using UnityEngine;
using DuksGames.Argon.Config;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Event;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    ///  Listen for 'acquire' events on the message bus. 
    /// </summary>
    public class InventoryEventListener : MonoBehaviour
    {
        IInventoryManager inventoryManager;

        GameObject _player => this.gameObject;

        void Start()
        {
            MessageBus.Instance.OnBroadcast.RemoveListener(this.HandleMessageBusEvent);
            MessageBus.Instance.OnBroadcast.AddListener(this.HandleMessageBusEvent);
            this.inventoryManager = SceneServices.Instance.InventoryManager;
        }

        void HandleMessageBusEvent(MessageBusEvent messageBusEvent)
        {
            if (messageBusEvent.CommandInfo.Initiator != this._player)
            {
                return;
            }

            if (messageBusEvent.Type.ToLower() == "acquire")
            {
                Debug.Log($"Inventory acquire: ");
                // Extract each target as an item from the events and feed them to 
                //  the player's inventory.
                foreach (var item in messageBusEvent.Targets)
                {
                    Debug.Log($"Inven: ac: {item.name}");
                    this.inventoryManager.Acquire(item);
                }
            }
        }


    }
}