using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public static class AddressablesBuildScript
    {
        public static string build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        public static string settings_asset
            = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        private static AddressableAssetSettings settings;

        public static string PlayerVersionOverride { get; private set; }

        public static AddressableAssetSettings GetSettingsObject(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            settings
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                as AddressableAssetSettings;

            if (settings == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                               $"a settings object.");
            return settings;
        }

        static void SetProfile(string profile)
        {
            string profileId = settings.profileSettings.GetProfileId(profile);
            if (string.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                 $"using current profile instead.");
            else
                settings.activeProfileId = profileId;
        }

        static void SetBuilder(IDataBuilder builder)
        {
            int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

            if (index > 0)
                settings.ActivePlayerDataBuilderIndex = index;
            else
                Debug.LogWarning($"{builder} must be added to the " +
                                 $"DataBuilders list before it can be made " +
                                 $"active. Using last run builder instead.");
        }

        static bool BuildAddressableContent()
        {
            AddressableAssetSettings
                .BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);

            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        public static bool BuildAddressables()
        {
            GetSettingsObject(settings_asset);

            if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) is not IDataBuilder builderScript)
            {
                Debug.LogError(build_script + " couldn't be found or isn't a build script.");
                return false;
            }

            SetBuilder(builderScript);

            PlayerVersionOverride = settings.OverridePlayerVersion;

            return BuildAddressableContent();
        }


        [MenuItem("Reflectis/Build Addressables")]
        public static void BuildAddressablesForAllPlatforms()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            BuildAddressables();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            BuildAddressables();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            BuildAddressables();
        }
    }
}
