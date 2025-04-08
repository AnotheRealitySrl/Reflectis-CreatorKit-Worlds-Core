using Reflectis.CreatorKit.Worlds.CoreEditor;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Unity.Properties;

using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using static Reflectis.CreatorKit.Worlds.Core.Editor.SceneListScriptableObject;

namespace Reflectis.CreatorKit.Worlds.Core.Editor
{
    public class AddressablesConfigurationWindowNew : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement root;

        private SceneListScriptableObject sceneConfigurations;

        private AddressableAssetSettings settings;

        private const string alphanumeric_string_pattern = @"^[a-zA-Z0-9]*$";
        private const string alphanumeric_lowercase_string_pattern = @"^[a-z0-9]*$";
        private const string alphanumeric_lowercase_string_pattern_negated = @"[^a-z0-9]";

        private const string addressables_output_folder = "ServerData";

        private const string remote_build_path_variable_name = "Remote.BuildPath";
        private const string remote_load_path_variable_name = "Remote.LoadPath";

        private const string build_target_variable_name = "BuildTarget";
        private const string build_target_variable_value = "[UnityEditor.EditorUserBuildSettings.activeBuildTarget]";
        private const string player_version_override_variable_name = "PlayerVersionOverride";
        private const string player_version_override_variable_value = "[Reflectis.CreatorKit.Worlds.CoreEditor.AddressablesBuildScript.PlayerVersionOverride]";

        private string remoteBuildPath;
        private string remoteLoadPath;

        private string playerVersionOverride;

        [CreateProperty] private string ActiveProfileName => settings.profileSettings.GetProfileName(settings.activeProfileId);

        [CreateProperty] private string CurrentRemoteBuildPath => settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name);
        [CreateProperty] private bool IsRemoteBuildPathConfigured => CurrentRemoteBuildPath == remoteBuildPath;

        [CreateProperty] private string CurrentRemoteLoadPath => settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name);
        [CreateProperty] private bool IsRemoteLoadPathConfigured => CurrentRemoteLoadPath == remoteLoadPath;

        [CreateProperty] private string CurrentBuildTarget => settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name);
        [CreateProperty] private bool IsBuildTargetConfigured => CurrentBuildTarget == build_target_variable_value;

        [CreateProperty] private string CurrentPlayerVersionOverride => settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name);
        [CreateProperty] private bool IsPlayerVersionOverrideConfigured => CurrentPlayerVersionOverride == player_version_override_variable_value;

        [CreateProperty] private bool AreAddressablesConfigured => IsAddressablesSettingsConfigured && IsProfileConfigured && AreAddressablesGroupsConfigured;

        [MenuItem("Reflectis/AddressablesConfigurationWindowNew")]
        public static void ShowExample()
        {
            AddressablesConfigurationWindowNew wnd = GetWindow<AddressablesConfigurationWindowNew>();
            wnd.titleContent = new GUIContent("AddressablesConfigurationWindow");

            wnd.LoadScenes();
            wnd.LoadAddressablesSettings();

            wnd.CreateDataBindings();
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

        }

        private void LoadScenes()
        {
            string addressablesBundleScriptableObjectsStr = AssetDatabase.FindAssets("t:" + typeof(SceneListScriptableObject).Name).ToList()[0];
            string path = AssetDatabase.GUIDToAssetPath(addressablesBundleScriptableObjectsStr);
            sceneConfigurations = AssetDatabase.LoadAssetAtPath<SceneListScriptableObject>(path);

            if (sceneConfigurations != null)
            {
                SerializedObject serializedObject = new(sceneConfigurations);
                SerializedProperty property = serializedObject.GetIterator();
                property.NextVisible(true);

                while (property.NextVisible(false))
                {
                    PropertyField propertyField = new(property);
                    propertyField.Bind(serializedObject);
                    root.Q<VisualElement>("scene-configuration-scriptable").Add(propertyField);
                }

                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                // Handle the case where sceneConfigurations is null
            }
        }

        private void LoadAddressablesSettings()
        {
            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);

            if (settings)
            {
                playerVersionOverride = settings.OverridePlayerVersion;

                remoteBuildPath = string.Join('/',
                    addressables_output_folder,
                    BuildtimeVariable(player_version_override_variable_name),
                    BuildtimeVariable(build_target_variable_name));

                var addressablesVariables = typeof(AddressablesVariables).GetProperties();
                string baseUrl = addressablesVariables.First(x => x.PropertyType == typeof(string)).Name;
                string worldId = addressablesVariables.First(x => x.PropertyType == typeof(int)).Name;
                remoteLoadPath = string.Join('/',
                    RuntimeVariable($"{typeof(AddressablesVariables)}.{baseUrl}"),
                    RuntimeVariable($"{typeof(AddressablesVariables)}.{worldId}"),
                    BuildtimeVariable(player_version_override_variable_name),
                    BuildtimeVariable(build_target_variable_name));
            }
        }

        private void CreateDataBindings()
        {
            VisualElement addressablesSettings = root.Q<VisualElement>("addressables-settings");
            addressablesSettings.dataSource = this;

            List<(string, string)> settingIcons = new()
            {
                { ("addressables-profile-check", nameof(IsAddressablesSettingsConfigured)) },
                { ("remote-buildpath-check", nameof(IsRemoteBuildPathConfigured)) },
                { ("remote-loadpath-check", nameof(IsRemoteLoadPathConfigured)) },
                { ("build-target-check", nameof(IsBuildTargetConfigured)) },
                { ("player-version-override-check", nameof(IsPlayerVersionOverrideConfigured)) },
                //{ ("project-settings-max-texture-size-check", nameof(projectConfig.MaxTextureSizeOverride)) },
            };
            foreach (var entry in settingIcons)
            {
                VisualElement projectSettingsItemIcon = addressablesSettings.Q<VisualElement>(entry.Item1);
                DataBinding styleBinding = new() { dataSourcePath = PropertyPath.FromName(entry.Item2) };
                styleBinding.sourceToUiConverters.AddConverter((ref bool value) =>
                {
                    projectSettingsItemIcon.RemoveFromClassList("settings-item-green-icon");
                    projectSettingsItemIcon.RemoveFromClassList("settings-item-red-icon");
                    projectSettingsItemIcon.AddToClassList(value ? "settings-item-green-icon" : "settings-item-red-icon");
                    return true;
                });
                // Find the binding that changes directly the class
                projectSettingsItemIcon.SetBinding(nameof(projectSettingsItemIcon.visible), styleBinding);
            }

            Label addressablesProfileValue = addressablesSettings.Q<Label>("addressables-profile-value");
            addressablesProfileValue.SetBinding(nameof(addressablesProfileValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(ActiveProfileName)),
                bindingMode = BindingMode.ToTarget
            });

            Label remoteBuildPathValue = addressablesSettings.Q<Label>("remote-buildpath-value");
            remoteBuildPathValue.SetBinding(nameof(remoteBuildPathValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(CurrentRemoteBuildPath)),
                bindingMode = BindingMode.ToTarget
            });

            Label remoteLoadPathValue = addressablesSettings.Q<Label>("remote-loadpath-value");
            remoteLoadPathValue.SetBinding(nameof(remoteLoadPathValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(CurrentRemoteLoadPath)),
                bindingMode = BindingMode.ToTarget
            });

            Label buildTargetValue = addressablesSettings.Q<Label>("build-target-value");
            buildTargetValue.SetBinding(nameof(buildTargetValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(CurrentBuildTarget)),
                bindingMode = BindingMode.ToTarget
            });

            Label playerVersionOverrideValue = addressablesSettings.Q<Label>("player-version-override-value");
            playerVersionOverrideValue.SetBinding(nameof(playerVersionOverrideValue.text), new DataBinding() { dataSourcePath = PropertyPath.FromName(nameof(CurrentPlayerVersionOverride)) });

            Button topLevelSettingsButton = root.Q<Button>("top-level-settings-button");
            topLevelSettingsButton.clicked += () => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Settings");

            Button profileSettingsButton = root.Q<Button>("profile-settings-button");
            profileSettingsButton.clicked += () => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");

            Button groupsSettingsButton = root.Q<Button>("default-local-group-button");
            groupsSettingsButton.clicked += () => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");

            Button buildAddressablesButton = root.Q<Button>("build-addressables-button");
            DataBinding buildAddressablesButtonDataBinding = new()
            {
                dataSourcePath = PropertyPath.FromName(nameof(AreAddressablesConfigured)),
                bindingMode = BindingMode.ToTarget
            };
            buildAddressablesButtonDataBinding.sourceToUiConverters.AddConverter((ref bool value) => AreAddressablesConfigured ? "Build Addressables" : "Fix Addressables Configurations");
            buildTargetValue.SetBinding(nameof(buildTargetValue.text), buildAddressablesButtonDataBinding);
            buildAddressablesButton.clicked += () =>
            {
                if (AreAddressablesConfigured)
                {
                    BuildSelectedAddressablesForAllPlatforms();
                }
                else
                {
                    ConfigureAddressablesSettings();
                    ConfigureProfile();
                    ConfigureAddressablesGroups();
                }
            };
        }

        #region Top-Level settings configuration


        [CreateProperty]
        public bool IsAddressablesSettingsConfigured =>
                 settings.RemoteCatalogLoadPath.GetName(settings) == remote_load_path_variable_name &&
                 settings.RemoteCatalogBuildPath.GetName(settings) == remote_build_path_variable_name &&
                 settings.BuildRemoteCatalog &&
                 settings.EnableJsonCatalog &&
                 settings.CheckForContentUpdateRestrictionsOption
                         == CheckForContentUpdateRestrictionsOptions.ListUpdatedAssetsWithRestrictions &&
                 settings.MaxConcurrentWebRequests == 3 &&
                 settings.CatalogRequestsTimeout == 0 &&
                 !settings.IgnoreUnsupportedFilesInBuild &&
                 !settings.UniqueBundleIds &&
                 settings.ContiguousBundles &&
                 settings.NonRecursiveBuilding &&
                 settings.BuiltInBundleNaming == BuiltInBundleNaming.Custom &&
                 settings.BuiltInBundleCustomNaming == playerVersionOverride &&
                 settings.MonoScriptBundleNaming == MonoScriptBundleNaming.Custom &&
                 settings.MonoScriptBundleCustomNaming == playerVersionOverride &&
                 !settings.DisableVisibleSubAssetRepresentations;


        private void ConfigureAddressablesSettings()
        {
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, remote_load_path_variable_name);
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, remote_build_path_variable_name);
            settings.BuildRemoteCatalog = true;
            settings.EnableJsonCatalog = true;
            settings.CheckForContentUpdateRestrictionsOption = CheckForContentUpdateRestrictionsOptions.ListUpdatedAssetsWithRestrictions;
            settings.ContentStateBuildPath = string.Empty;
            settings.MaxConcurrentWebRequests = 3;
            settings.CatalogRequestsTimeout = 0;
            settings.IgnoreUnsupportedFilesInBuild = false;
            settings.UniqueBundleIds = false;
            settings.ContiguousBundles = true;
            settings.NonRecursiveBuilding = true;
            settings.BuiltInBundleNaming = BuiltInBundleNaming.Custom;
            settings.BuiltInBundleCustomNaming = playerVersionOverride;
            settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Custom;
            settings.MonoScriptBundleCustomNaming = playerVersionOverride;
            settings.DisableVisibleSubAssetRepresentations = false;
            settings.BuildRemoteCatalog = true;

            SaveSettings();
        }

        private void ConfigureAddressablesSettingsForBuild()
        {
            settings.BuiltInBundleCustomNaming = playerVersionOverride;
            settings.BuiltInBundleCustomNaming = playerVersionOverride;
        }

        #endregion

        #region Profiles configuration

        [CreateProperty]
        private bool IsProfileConfigured =>
            settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name) == remoteBuildPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name) == remoteLoadPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name) == build_target_variable_value
                && settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name) == player_version_override_variable_value;


        private void ConfigureProfile()
        {
            settings.profileSettings.SetValue(settings.activeProfileId, remote_build_path_variable_name, remoteBuildPath);
            settings.profileSettings.SetValue(settings.activeProfileId, remote_load_path_variable_name, remoteLoadPath);

            if (settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name) == null)
            {
                settings.profileSettings.CreateValue(build_target_variable_name, build_target_variable_value);
            }
            else
            {
                settings.profileSettings.SetValue(settings.activeProfileId, build_target_variable_name, build_target_variable_value);
            }

            if (settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name) == null)
            {
                settings.profileSettings.CreateValue(player_version_override_variable_name, player_version_override_variable_value);
            }
            else
            {
                settings.profileSettings.SetValue(settings.activeProfileId, player_version_override_variable_name, player_version_override_variable_value);
            }

            SaveSettings();
        }

        #endregion

        #region Groups configuration

        [CreateProperty]
        private bool AreAddressablesGroupsConfigured
        {
            get
            {
                bool configured = true;
                settings.groups.ForEach(group => configured &= IsAddresableGroupConfigured(group));
                return configured;
            }
        }

        private bool IsAddresableGroupConfigured(AddressableAssetGroup group)
        {
            bool configured = true;

            group.Schemas.Where(schema => schema is BundledAssetGroupSchema).ToList().ForEach(schema =>
            {
                BundledAssetGroupSchema bundledAssetGroupSchema = schema as BundledAssetGroupSchema;

                configured &=
                    bundledAssetGroupSchema.LoadPath.GetName(settings) == remote_load_path_variable_name &&
                    bundledAssetGroupSchema.BuildPath.GetName(settings) == remote_build_path_variable_name &&
                    bundledAssetGroupSchema.Compression == BundledAssetGroupSchema.BundleCompressionMode.LZ4 &&
                    bundledAssetGroupSchema.IncludeInBuild == true &&
                    bundledAssetGroupSchema.ForceUniqueProvider == false &&
                    bundledAssetGroupSchema.UseAssetBundleCache == true &&
                    bundledAssetGroupSchema.UseAssetBundleCrc == false &&
                    bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles == false &&
                    bundledAssetGroupSchema.UseUnityWebRequestForLocalBundles == false &&
                    bundledAssetGroupSchema.Timeout == 0 &&
                    bundledAssetGroupSchema.ChunkedTransfer == false &&
                    bundledAssetGroupSchema.RedirectLimit == -1 &&
                    bundledAssetGroupSchema.RetryCount == 0 &&
                    bundledAssetGroupSchema.IncludeAddressInCatalog == true &&
                    bundledAssetGroupSchema.IncludeGUIDInCatalog == true &&
                    bundledAssetGroupSchema.IncludeLabelsInCatalog == true &&
                    bundledAssetGroupSchema.InternalIdNamingMode == BundledAssetGroupSchema.AssetNamingMode.FullPath &&
                    bundledAssetGroupSchema.InternalBundleIdMode == BundledAssetGroupSchema.BundleInternalIdMode.GroupGuidProjectIdHash &&
                    bundledAssetGroupSchema.AssetBundledCacheClearBehavior == BundledAssetGroupSchema.CacheClearBehavior.ClearWhenWhenNewVersionLoaded &&
                    bundledAssetGroupSchema.BundleMode == BundledAssetGroupSchema.BundlePackingMode.PackSeparately &&
                    bundledAssetGroupSchema.BundleNaming == BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            });

            return configured;
        }

        private void ConfigureAddressablesGroups()
        {
            settings.groups.ForEach(group =>
            {
                group.Schemas.Where(schema => schema is BundledAssetGroupSchema).ToList().ForEach(schema =>
                {
                    BundledAssetGroupSchema bundledAssetGroupSchema = schema as BundledAssetGroupSchema;

                    bundledAssetGroupSchema.LoadPath.SetVariableByName(settings, remote_load_path_variable_name);
                    bundledAssetGroupSchema.BuildPath.SetVariableByName(settings, remote_build_path_variable_name);
                    bundledAssetGroupSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                    bundledAssetGroupSchema.IncludeInBuild = true;
                    bundledAssetGroupSchema.ForceUniqueProvider = false;
                    bundledAssetGroupSchema.UseAssetBundleCache = true;
                    bundledAssetGroupSchema.UseAssetBundleCrc = false;
                    bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = false;
                    bundledAssetGroupSchema.UseUnityWebRequestForLocalBundles = false;
                    bundledAssetGroupSchema.Timeout = 0;
                    bundledAssetGroupSchema.ChunkedTransfer = false;
                    bundledAssetGroupSchema.RedirectLimit = -1;
                    bundledAssetGroupSchema.RetryCount = 0;
                    bundledAssetGroupSchema.IncludeAddressInCatalog = true;
                    bundledAssetGroupSchema.IncludeGUIDInCatalog = true;
                    bundledAssetGroupSchema.IncludeLabelsInCatalog = true;
                    bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;
                    bundledAssetGroupSchema.InternalBundleIdMode = BundledAssetGroupSchema.BundleInternalIdMode.GroupGuidProjectIdHash;
                    bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenWhenNewVersionLoaded;
                    bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                    bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                });
            });

            SaveSettings();
        }

        #endregion

        private string BuildtimeVariable(string variable) => "[" + variable + "]";
        private string RuntimeVariable(string variable) => "{" + variable + "}";

        private void SaveSettings()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        #region Test

        private void BuildSelectedAddressablesForAllPlatforms()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            BuildAddressablesForSelectedPlatform();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            BuildAddressablesForSelectedPlatform();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            BuildAddressablesForSelectedPlatform();
        }

        private void BuildAddressablesForSelectedPlatform()
        {
            AddressableAssetGroup assetGroup = settings.DefaultGroup;
            foreach (AddressableAssetEntry entry in assetGroup.entries.Where(x => x.IsScene))
            {
                bool success = settings.RemoveAssetEntry(entry.guid);
            }

            foreach (var scene in sceneConfigurations.SceneConfigurations)
            {
                if (scene.IncludeInBuild)
                {
                    BuildAddressablesForTargetGroup(scene);
                }
            }
        }

        private void BuildAddressablesForTargetGroup(SceneConfiguration configuration)
        {
            AddressableAssetGroup assetGroup = settings.DefaultGroup;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(configuration.Scene, out string guid, out long _);
            AddressableAssetEntry addressableEntry = settings.CreateOrMoveEntry(guid, assetGroup);

            string addressableAddress = Regex.Replace(configuration.Scene.name.ToLower(), alphanumeric_lowercase_string_pattern_negated, string.Empty);
            addressableEntry.SetAddress(addressableAddress);

            settings.OverridePlayerVersion = addressableAddress;
            ConfigureAddressablesSettingsForBuild();

            AddressablesBuildScript.BuildAddressables();

            settings.RemoveAssetEntry(guid);
        }

        #endregion

    }
}