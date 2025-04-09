using Reflectis.CreatorKit.Worlds.CoreEditor;

using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private enum EBuildError
        {
            None,
            FolderMissing,
            BinaryCatalog,
        }

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement root;

        private SceneListScriptableObject sceneConfigurations;

        private AddressableAssetSettings settings;

        private const string scenes_settings_path = "Assets/ReflectisConfiguration/Addressables/AddressablesSceneList.asset";


        private const string addressables_output_folder = "ServerData";

        private const string remote_build_path_variable_name = "Remote.BuildPath";
        private const string remote_load_path_variable_name = "Remote.LoadPath";

        private const string build_target_variable_name = "BuildTarget";
        private const string build_target_variable_value = "[UnityEditor.EditorUserBuildSettings.activeBuildTarget]";
        private const string player_version_override_variable_name = "PlayerVersionOverride";
        private const string player_version_override_variable_value = "[Reflectis.CreatorKit.Worlds.CoreEditor.AddressablesBuildScript.PlayerVersionOverride]";

        private string remoteBuildPath;
        private string remoteLoadPath;

        [SerializeField] private EBuildError buildResult = EBuildError.None;

        [CreateProperty] private string ActiveProfileName => settings.profileSettings.GetProfileName(settings.activeProfileId);

        [CreateProperty] private string CurrentRemoteBuildPathVariableValue => settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name);
        [CreateProperty] private bool IsRemoteBuildPathConfigured => CurrentRemoteBuildPathVariableValue == remoteBuildPath;

        [CreateProperty] private string CurrentRemoteLoadPathVariableValue => settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name);
        [CreateProperty] private bool IsRemoteLoadPathConfigured => CurrentRemoteLoadPathVariableValue == remoteLoadPath;

        [CreateProperty] private string CurrentBuildTargetVariableValue => settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name);
        [CreateProperty] private bool IsBuildTargetConfigured => CurrentBuildTargetVariableValue == build_target_variable_value;

        [CreateProperty] private string CurrentPlayerVersionOverrideVariableValue => settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name);
        [CreateProperty] private bool IsPlayerVersionOverrideConfigured => CurrentPlayerVersionOverrideVariableValue == player_version_override_variable_value;

        [CreateProperty] private bool AreAddressablesConfigured => IsAddressablesSettingsConfigured && IsProfileConfigured && AreAddressablesGroupsConfigured;


        [MenuItem("Reflectis/AddressablesConfigurationWindowNew")]
        public static void ShowExample()
        {
            AddressablesConfigurationWindowNew wnd = GetWindow<AddressablesConfigurationWindowNew>();
            wnd.titleContent = new GUIContent("AddressablesConfigurationWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            InitializeWindow();
        }

        private void OnEnable()
        {
            // Registra la callback per l'importazione dei pacchetti
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            // Registra la callback per l'aggiornamento dell'editor
            //EditorApplication.update += InitializeWindow;
        }

        private void OnDisable()
        {
            // Rimuovi la callback per l'importazione dei pacchetti
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            // Rimuovi la callback per l'aggiornamento dell'editor
            //EditorApplication.update -= InitializeWindow;
        }

        private void OnImportPackageCompleted(string packageName)
        {
            // Esegui il refresh della finestra
            InitializeWindow();
        }

        private void InitializeWindow()
        {

            LoadSettings();
            CreateDataBindings();
        }

        private void LoadSettings()
        {
            string addressablesBundleScriptableObjectsStr = AssetDatabase.FindAssets("t:" + typeof(SceneListScriptableObject).Name).ToList().FirstOrDefault();
            string path = addressablesBundleScriptableObjectsStr != null ? AssetDatabase.GUIDToAssetPath(addressablesBundleScriptableObjectsStr) : scenes_settings_path;
            sceneConfigurations = AssetDatabase.LoadAssetAtPath<SceneListScriptableObject>(path);

            if (sceneConfigurations == null)
            {
                sceneConfigurations = CreateInstance<SceneListScriptableObject>();
                AssetDatabase.CreateAsset(sceneConfigurations, scenes_settings_path);
                AssetDatabase.SaveAssets();
            }

            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);

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

        private void CreateDataBindings()
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

            VisualElement addressablesSettings = root.Q<VisualElement>("addressables-settings");
            addressablesSettings.dataSource = this;

            List<(string, string)> settingIcons = new()
            {
                { ("addressables-profile-check", nameof(IsAddressablesSettingsConfigured)) },
                { ("remote-buildpath-check", nameof(IsRemoteBuildPathConfigured)) },
                { ("remote-loadpath-check", nameof(IsRemoteLoadPathConfigured)) },
                { ("build-target-check", nameof(IsBuildTargetConfigured)) },
                { ("player-version-override-check", nameof(IsPlayerVersionOverrideConfigured)) },
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
                dataSourcePath = PropertyPath.FromName(nameof(CurrentRemoteBuildPathVariableValue)),
                bindingMode = BindingMode.ToTarget
            });

            Label remoteLoadPathValue = addressablesSettings.Q<Label>("remote-loadpath-value");
            remoteLoadPathValue.SetBinding(nameof(remoteLoadPathValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(CurrentRemoteLoadPathVariableValue)),
                bindingMode = BindingMode.ToTarget
            });

            Label buildTargetValue = addressablesSettings.Q<Label>("build-target-value");
            buildTargetValue.SetBinding(nameof(buildTargetValue.text), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(CurrentBuildTargetVariableValue)),
                bindingMode = BindingMode.ToTarget
            });

            Label playerVersionOverrideValue = addressablesSettings.Q<Label>("player-version-override-value");
            playerVersionOverrideValue.SetBinding(nameof(playerVersionOverrideValue.text), new DataBinding() { dataSourcePath = PropertyPath.FromName(nameof(CurrentPlayerVersionOverrideVariableValue)) });

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


            VisualElement buildErrors = root.Q<VisualElement>("build-result-errors");
            buildErrors.dataSource = this;

            VisualElement folderMissing = buildErrors.Q<VisualElement>("folder-missing");
            DataBinding folderMissingDataBinding = new()
            {
                dataSourcePath = PropertyPath.FromName(nameof(buildResult)),
                bindingMode = BindingMode.ToTarget
            };
            folderMissingDataBinding.sourceToUiConverters.AddConverter((ref EBuildError value) => buildResult == EBuildError.FolderMissing);
            folderMissing.SetBinding(nameof(folderMissing.visible), folderMissingDataBinding);

            VisualElement binaryCatalog = buildErrors.Q<VisualElement>("binary-catalog");
            DataBinding binaryCatalogDataBinding = new()
            {
                dataSourcePath = PropertyPath.FromName(nameof(buildResult)),
                bindingMode = BindingMode.ToTarget
            };
            binaryCatalogDataBinding.sourceToUiConverters.AddConverter((ref EBuildError value) => buildResult == EBuildError.BinaryCatalog);
            binaryCatalog.SetBinding(nameof(binaryCatalog.visible), binaryCatalogDataBinding);
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
                 settings.MonoScriptBundleNaming == MonoScriptBundleNaming.Custom &&
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
            settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Custom;
            settings.DisableVisibleSubAssetRepresentations = false;
            settings.BuildRemoteCatalog = true;

            SaveSettings();
        }

        private void ConfigureAddressablesSettingsForBuild()
        {
            settings.BuiltInBundleCustomNaming = settings.OverridePlayerVersion;
            settings.MonoScriptBundleCustomNaming = settings.OverridePlayerVersion;
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
                settings.groups.ForEach(group => configured &= IsAddressableGroupConfigured(group));
                return configured;
            }
        }

        private bool IsAddressableGroupConfigured(AddressableAssetGroup group)
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

            buildResult = CheckBuildResult();
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

            string sceneNameFiltered = configuration.SceneNameFiltered;
            addressableEntry.SetAddress(sceneNameFiltered);

            settings.OverridePlayerVersion = sceneNameFiltered;
            ConfigureAddressablesSettingsForBuild();

            AddressablesBuildScript.BuildAddressables();

            settings.RemoveAssetEntry(guid);

            settings.OverridePlayerVersion = string.Empty;
        }

        private EBuildError CheckBuildResult()
        {
            List<string> builtScenes = sceneConfigurations.SceneConfigurations
                .Where(x => x.IncludeInBuild)
                .Select(x => x.SceneNameFiltered)
                .ToList();

            foreach (string scene in builtScenes)
            {
                string sceneFolderPath = Path.Combine(addressables_output_folder, scene);
                string[] requiredSubfolders = { "WebGL", "StandaloneWindows64", "Android" };

                foreach (string subfolder in requiredSubfolders)
                {
                    string subfolderPath = Path.Combine(sceneFolderPath, subfolder);
                    if (!Directory.Exists(subfolderPath))
                    {
                        Debug.LogError($"Subfolder {subfolder} not found in {sceneFolderPath}");
                        return EBuildError.FolderMissing;
                    }

                    bool catalogFileExists = Directory.GetFiles(subfolderPath, "*catalog*.json").Any();
                    if (!catalogFileExists)
                    {
                        Debug.LogError($"Catalog file not found in {subfolderPath}");
                        return EBuildError.BinaryCatalog;
                    }
                }
            }

            return EBuildError.None;
        }

        #endregion

    }
}