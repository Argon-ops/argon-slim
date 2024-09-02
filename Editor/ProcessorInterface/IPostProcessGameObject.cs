using UnityEngine;
using UnityEditor;

namespace DuksGames.Tools
{
    // for classes that want a callback from 
    // OnPostprocessGameObjectWithUserProperties(GameObject go, string[] propNames, System.Object[] values )
    public interface IPostProcessWithProperties
    {
        void PostProcessWithProperties(GameObject go, string[] propNames, System.Object[] values);
    }


}