using UnityEngine;
using UnityEngine.Events;

namespace DuksGames.Argon.Event
{
    public class MessageBus : MonoBehaviour
    {
        public UnityEvent<MessageBusEvent> OnBroadcast;

        public void Broadcast(MessageBusEvent messageBusEvent)
        {
            this.OnBroadcast.Invoke(messageBusEvent);
        }

        static MessageBus _instance;
        public static MessageBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MessageBus>();
                }
                return _instance;
            }
        }
    }
}