using UnityEngine;

namespace DuksGames.Argon.Utils
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        #region  Fields
        private static T _instance;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        [SerializeField]
        private bool _persistent = false;

        #endregion

        #region  Properties
        public static T Instance
        {
            get
            {
                // if (Quitting)
                // {
                //     Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                //     // ReSharper disable once AssignNullToNotNullAttribute
                //     return null;
                // }
                // lock (Lock)
                // {
                if (_instance != null)
                    return _instance;
                var instances = FindObjectsOfType<T>();
                var count = instances.Length;
                if (count > 0)
                {
                    if (count == 1)
                        return _instance = instances[0];
                    Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                    for (var i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);
                    return _instance = instances[0];
                }

                Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}")
                           .AddComponent<T>();
                // }
            }
        }
        #endregion

        #region  Methods
        protected override void _Awake()
        {
            // Debug.Log($"<color='#FFFFaa'> Singleton <T> awake </color> Quitting is: {Quitting}");

            if (_persistent)
                DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake() { }
        #endregion
    }

    public abstract class Singleton : MonoBehaviour
    {
        #region  Properties
        // NOTE: we think we want to track Quitting. Seems smart...
        //  but when domain reloading is off. (So game loads faster in editor)
        //    Quitting persists, stays true. So this singleton nevers creates itself on the second play (unless you recompile which defeats the
        //      entire purpose, doesn't it.)
        // public static bool Quitting { get; private set; }
        #endregion

        #region  Methods


        void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            // if domain reloading is off, Quitting will persist; and be true when we next hit Play in the editor
            // Debug.Log($"<color='#88FFDD'> Singleton awake </color> Quitting was: {Quitting}");
            // Quitting = false;
            this._Awake();
        }

        protected virtual void _Awake() { }

        private void OnApplicationQuit()
        {
            // Quitting = true;
            // Debug.Log($"<color='#88ddFF'> Singleton Quit </color> Quitting is: {Quitting}");
        }
        #endregion
    }
}