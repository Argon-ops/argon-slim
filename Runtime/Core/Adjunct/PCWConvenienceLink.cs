using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DuksGames.Argon.Animate;

namespace DuksGames.Argon.Core
{

    public class PCWConvenienceLink : MonoBehaviour
    {
        [Tooltip("Convenient links to PlayableClipWrapper that pertain to this object.")] public List<PlayableClipWrapper> WrapperLinks = new();
    }
}