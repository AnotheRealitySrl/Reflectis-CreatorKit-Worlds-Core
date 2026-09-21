using UnityEditor;


using SPACS.Editor;

namespace Virtuademy.SDK.Environments.Tasks.Editor
{
    [InitializeOnLoad]
    public class ScriptDefineSymbols
    {
        public const string TASKS_SCRIPT_DEFINE_SYMBOL = "VIRTUADEMY_ENVIRONMENTS_TASKS";
        static ScriptDefineSymbols()
        {
            // The symbol was REFLECTIS_CREATOR_KIT_WORLDS_* until 2026-09-21; a project updating the package
            // still carries the old one in its PlayerSettings, so it is retired here, once, before the new one is added.
            ScriptDefineSymbolsUtilities.RemoveScriptingDefineSymbolFromAllBuildTargetGroups(TASKS_SCRIPT_DEFINE_SYMBOL.Replace("VIRTUADEMY_ENVIRONMENTS_", "REFLECTIS_CREATOR_KIT_WORLDS_"));
            ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(TASKS_SCRIPT_DEFINE_SYMBOL);
        }
    }
}