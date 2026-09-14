using UnityEditor;


using SPACS.Editor;

namespace Virtuademy.SDK.Environments.VisualScripting.Editor
{
    [InitializeOnLoad]
    public class ScriptDefineSymbols
    {
        public const string VISUAL_SCRIPTING_SCRIPT_DEFINE_SYMBOL = "REFLECTIS_CREATOR_KIT_WORLDS_VISUAL_SCRIPTING";
        static ScriptDefineSymbols()
        {
            ScriptDefineSymbolsUtilities.AddScriptingDefineSymbolToAllBuildTargetGroups(VISUAL_SCRIPTING_SCRIPT_DEFINE_SYMBOL);
        }
    }
}