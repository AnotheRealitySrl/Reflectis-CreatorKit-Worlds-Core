using Reflectis.CreatorKit.Worlds.CoreEditor;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

namespace Reflectis.CreatorKit.Worlds.Installer.Editor
{
    public static class BreakingChangesSolver2025_3
    {
        [MenuItem("Reflectis/Creator Kit update routines/Load AddressablesBundleScriptableObjects")]
        public static void LoadAddressablesBundleScriptableObjects()
        {
            string[] guids = AssetDatabase.FindAssets("t:AddressablesBundleScriptableObject", new[] { "Assets/ReflectisSettings/Editor/AddressablesConfiguration" });
            List<AddressablesBundleScriptableObject> scriptableObjects = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<AddressablesBundleScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            foreach (var scriptableObject in scriptableObjects)
            {
                //scriptableObject.environmentasset.Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneConfig.SceneNameFiltered));

                EditorUtility.SetDirty(scriptableObject);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
