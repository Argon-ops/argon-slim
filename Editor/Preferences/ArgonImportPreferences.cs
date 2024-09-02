using UnityEngine;


namespace DuksGames.Tools
{
    [CreateAssetMenu(fileName = "ArgonImportPreferences", menuName = "Argon/Import Preferences")]
    public class ArgonImportPreferences : ScriptableObject
    {
        public bool IsArgonEnabled = true;
    }
}
