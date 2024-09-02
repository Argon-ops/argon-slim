using System.Threading.Tasks;
using UnityEngine;

namespace DuksGames.Argon.Adapters
{
    public interface ICameraShake
    {
        Task Shake(CameraShakeConfig shakeConfig);
        void StopShaking();
    }

    [System.Serializable]
    public struct CameraShakeConfig
    {
        public float ShakeDuration;
        public float DisplacementDistance;
    }
}