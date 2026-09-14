using UnityEditor;


using SPACS.Editor;

namespace Virtuademy.SDK.Environments.Analytics.Editor
{
    [InitializeOnLoad]
    public class ScriptDefineSymbols
    {
        public const string ANALYTICS_SCRIPT_DEFINE_SYMBOL = "REFLECTIS_CREATOR_KIT_WORLDS_ANALYTICS";
        static ScriptDefineSymbols()
        {
            ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(ANALYTICS_SCRIPT_DEFINE_SYMBOL);
        }
    }
}