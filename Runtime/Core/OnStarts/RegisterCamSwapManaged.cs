using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{
    public class RegisterCamSwapManaged : MonoBehaviour
    {
        void Start()
        {
            var camera = this.GetComponent<Camera>();
            Assert.IsFalse(camera == null);
            var manager = ShGameObjectHelper.FindInScene<ICamSwapManager>(); 
            manager.AddManaged(camera);

            Destroy(this);
        }
    }
}
