using SPACS.Editor;

using UnityEditor;


[InitializeOnLoad]
public class EnvironmentVariables
{
    public const string TASKS_ENV_VARIABLE = "VIRTUADEMY_ENVIRONMENTS_TASKS";
    static EnvironmentVariables()
    {
        // The symbol was REFLECTIS_CREATOR_KIT_WORLDS_* until 2026-09-21; a project updating the package
        // still carries the old one in its PlayerSettings, so it is retired here, once, before the new one is added.
        ScriptDefineSymbolsUtilities.RemoveScriptingDefineSymbolFromAllBuildTargetGroups(TASKS_ENV_VARIABLE.Replace("VIRTUADEMY_ENVIRONMENTS_", "REFLECTIS_CREATOR_KIT_WORLDS_"));
        ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(TASKS_ENV_VARIABLE);
    }
}
