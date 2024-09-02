using UnityEngine;
using UnityEditor;

namespace DuksGames.Tools
{

    public static class GlobalsConsumersList
    {
        public static AbstractGlobalsImportSettingsConsumer[] Consumers = new AbstractGlobalsImportSettingsConsumer[] {
            ScriptableObject.CreateInstance<PCWrapperGenerator>(),
        };
    }

    public abstract class AbstractGlobalsImportSettingsConsumer : ScriptableObject
    {
        public abstract void ConsumeGlobals(GameObject root, IntermediateProductSet intermediateProductSet);
    }
}