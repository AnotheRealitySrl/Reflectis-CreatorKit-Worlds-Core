using Virtuademy.SDK.Core.Editor;
using UnityEditor;

namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    [InitializeOnLoad]
    public class ScriptDefineSymbols
    {
        public const string PLACEHOLDERS_SCRIPT_DEFINE_SYMBOL = "REFLECTIS_CREATOR_KIT_WORLDS_PLACEHOLDERS";
        static ScriptDefineSymbols()
        {
            ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(PLACEHOLDERS_SCRIPT_DEFINE_SYMBOL);
        }
    }
}