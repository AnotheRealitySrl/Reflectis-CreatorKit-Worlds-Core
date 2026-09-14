using UnityEditor;


using SPACS.Editor;

namespace Virtuademy.SDK.Environments.Tasks.Editor
{
    [InitializeOnLoad]
    public class ScriptDefineSymbols
    {
        public const string TASKS_SCRIPT_DEFINE_SYMBOL = "REFLECTIS_CREATOR_KIT_WORLDS_TASKS";
        static ScriptDefineSymbols()
        {
            ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(TASKS_SCRIPT_DEFINE_SYMBOL);
        }
    }
}