using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;

namespace DuksGames.Argon.Config
{
    public class SceneServices : MonoBehaviour
    {
        [SerializeField] GameObject _CursorLock;
        public ICursorLocker CursorLocker => _CursorLock.GetComponent<ICursorLocker>();

        [SerializeField] GameObject _CamSwap;
        public ICamSwap CamSwap => _CamSwap.GetComponent<ICamSwap>();

        [SerializeField] GameObject _PlayerProvider;
        public IPlayerProvider PlayerProvider => _PlayerProvider.GetComponent<IPlayerProvider>();

        [SerializeField] GameObject _Inventory;
        public IInventoryManager InventoryManager => _Inventory.GetComponent<IInventoryManager>();

        [SerializeField] GameObject _OneAtATimeHighlightManager;
        public IOneAtATimeHighlightManager OneAtATimeHighlightManager =>
                    _OneAtATimeHighlightManager.GetComponent<IOneAtATimeHighlightManager>();

        [SerializeField] GameObject _CamLockSessionOneAtATimeHighlightManager;
        public ICamLockSessionOneAtATimeHighlightManager CamLockSessionOneAtATimeHighlightManager =>
                    _CamLockSessionOneAtATimeHighlightManager.GetComponent<ICamLockSessionOneAtATimeHighlightManager>();

        [SerializeField] GameObject _HeadlineDisplay;
        public IHeadlineDisplay HeadlineDisplay => _HeadlineDisplay.GetComponent<IHeadlineDisplay>();

        [SerializeField] GameObject _PlayerUpdateStack;
        public IUpdateStack PlayerUpdateStack => _PlayerUpdateStack.GetComponent<IUpdateStack>();

        [SerializeField] GameObject _ThirdPersonControllerStateManager;
        public IThirdPersonControllerStateManager ThirdPersonControllerStateManager =>
                    _ThirdPersonControllerStateManager.GetComponent<IThirdPersonControllerStateManager>();

        // RIP: Proximity
        // [SerializeField] GameObject _ProximityClickManager;
        // public IProximityClickManager ProximityClickManager =>
        //             _ProximityClickManager.GetComponent<IProximityClickManager>();

        [SerializeField] GameObject _CurrentBeaconProvider;
        public ICurrentBeaconProvider CurrentBeaconProvider =>
                    _CurrentBeaconProvider.GetComponent<ICurrentBeaconProvider>();

        [SerializeField] GameObject _HUDStateManager;
        public IHUDStateManager HUDStateManager => _HUDStateManager.GetComponent<IHUDStateManager>();
        public IHUDStateEventDispatcher HUDStateEventDispatcher => 
                        _HUDStateManager.GetComponent<IHUDStateEventDispatcher>();



        static SceneServices _instance;
        public static SceneServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SceneServices>();
                }
                return _instance;
            }
        }

    }

}