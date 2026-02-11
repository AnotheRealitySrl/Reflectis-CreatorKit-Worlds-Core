using Newtonsoft.Json;
using Reflectis.CreatorKit.Worlds.CoreEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.Properties;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using static Reflectis.CreatorKit.Worlds.Core.Editor.SceneListScriptableObject;

namespace Reflectis.CreatorKit.Worlds.Core.Editor
{
    public class AddressablesManagementWindow : EditorWindow
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

        private const string settings_folder_path = "Assets/CreatorKit/Editor/Settings";
        private const string addressables_configuration_file = "AddressablesSceneList.asset";

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


        private static HttpClient httpClient = new HttpClient();

        public static string token = "";

        [MenuItem("Reflectis Worlds/Creator Kit/Core/Addressables management")]
        public static void ShowExample()
        {
            AddressablesManagementWindow wnd = GetWindow<AddressablesManagementWindow>();
            wnd.titleContent = new GUIContent("Addressables management");
        }

        private void OnApplicationQuit()
        {
            SaveAsset(sceneConfigurations);
        }

        private void OnDestroy()
        {
            SaveAsset(sceneConfigurations);
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

        private void InitializeWindow()
        {
            LoadSettings();
            AddDataBindings();
        }

        private void LoadSettings()
        {
            string addressablesBundleScriptableObjectsStr = AssetDatabase.FindAssets("t:" + typeof(SceneListScriptableObject).Name).ToList().FirstOrDefault();
            sceneConfigurations = AssetDatabase.LoadAssetAtPath<SceneListScriptableObject>(AssetDatabase.GUIDToAssetPath(addressablesBundleScriptableObjectsStr));

            if (sceneConfigurations == null)
            {
                EnsureFolderExists(settings_folder_path);

                sceneConfigurations = CreateInstance<SceneListScriptableObject>();
                string settingsAssetPath = $"{settings_folder_path}/{addressables_configuration_file}";
                AssetDatabase.CreateAsset(sceneConfigurations, settingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);

            if (!settings)
            {
                AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                               AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);
            }

            remoteBuildPath = string.Join('/',
                addressables_output_folder,
                BuildtimeVariable(player_version_override_variable_name),
                BuildtimeVariable(build_target_variable_name));

            var addressablesVariables = typeof(AddressablesVariables).GetProperties();
            string baseUrl = addressablesVariables[0].Name;
            string worldId = addressablesVariables[1].Name;
            remoteLoadPath = string.Join('/',
                RuntimeVariable($"{typeof(AddressablesVariables)}.{baseUrl}"),
                RuntimeVariable($"{typeof(AddressablesVariables)}.{worldId}"),
                BuildtimeVariable(player_version_override_variable_name),
                BuildtimeVariable(build_target_variable_name));
        }

        private void AddDataBindings()
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
                { ("default-local-group-check", nameof(AreAddressablesConfigured)) },
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

            Button groupsSettingsButton = root.Q<Button>("addressables-groups-button");
            groupsSettingsButton.clicked += () => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");

            Button defaultLocalGroupButton = root.Q<Button>("default-local-group-button");
            defaultLocalGroupButton.clicked += () => Selection.activeObject = settings.DefaultGroup;


            Button buildAddressablesButton = root.Q<Button>("build-addressables-button");
            buildAddressablesButton.dataSource = this;
            DataBinding buildAddressablesButtonDataBinding = new()
            {
                dataSourcePath = PropertyPath.FromName(nameof(AreAddressablesConfigured)),
                bindingMode = BindingMode.ToTarget
            };
            buildAddressablesButtonDataBinding.sourceToUiConverters.AddConverter((ref bool value) => AreAddressablesConfigured ? "Build Addressables" : "Fix Addressables configurations");
            buildAddressablesButton.SetBinding(nameof(buildAddressablesButton.text), buildAddressablesButtonDataBinding);
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
            folderMissingDataBinding.sourceToUiConverters.AddConverter((ref EBuildError value) =>
            {
                folderMissing.style.display = buildResult == EBuildError.FolderMissing ? DisplayStyle.Flex : DisplayStyle.None;
                return true;
            });
            folderMissing.SetBinding(nameof(folderMissing.visible), folderMissingDataBinding);

            VisualElement binaryCatalog = buildErrors.Q<VisualElement>("binary-catalog");
            DataBinding binaryCatalogDataBinding = new()
            {
                dataSourcePath = PropertyPath.FromName(nameof(buildResult)),
                bindingMode = BindingMode.ToTarget
            };
            binaryCatalogDataBinding.sourceToUiConverters.AddConverter((ref EBuildError value) =>
            {
                binaryCatalog.style.display = buildResult == EBuildError.BinaryCatalog ? DisplayStyle.Flex : DisplayStyle.None;
                return true;
            });
            binaryCatalog.SetBinding(nameof(folderMissing.visible), binaryCatalogDataBinding);
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

            SaveAsset(settings);
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

            SaveAsset(settings);
        }

        #endregion

        #region Groups configuration

        [CreateProperty]
        private bool AreAddressablesGroupsConfigured
        {
            get
            {
                bool configured = true;

                var defaultGroup = settings.DefaultGroup;

                configured &= !settings.groups.Where(group => group != defaultGroup).Any();
                configured &= !defaultGroup.entries.Any();

                defaultGroup.Schemas.Where(schema => schema is BundledAssetGroupSchema).ToList().ForEach(schema =>
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
        }

        private void ConfigureAddressablesGroups()
        {
            var defaultGroup = settings.DefaultGroup;

            // Collect all groups except the default group
            var groupsToDelete = settings.groups.Where(group => group != defaultGroup).ToList();

            // Delete each group
            foreach (var group in groupsToDelete)
            {
                settings.RemoveGroup(group);
            }

            defaultGroup.Schemas.Where(schema => schema is BundledAssetGroupSchema).ToList().ForEach(schema =>
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

                SaveAsset(bundledAssetGroupSchema);
            });

            var entriesToRemove = defaultGroup.entries.ToList();
            foreach (var entry in entriesToRemove)
            {
                settings.RemoveAssetEntry(entry.guid);
            }

            SaveAsset(settings);

            // Save changes
            SaveAsset(defaultGroup);
            SaveAsset(settings);
        }

        #endregion

        private string BuildtimeVariable(string variable) => "[" + variable + "]";
        private string RuntimeVariable(string variable) => "{" + variable + "}";

        private void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }

        private void EnsureFolderExists(string folderPath)
        {
            string[] folders = folderPath.Split('/');
            string currentPath = "";

            foreach (string folder in folders)
            {
                currentPath = Path.Combine(currentPath, folder);
                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(currentPath), Path.GetFileName(currentPath));
                }
            }
        }


        #region Build

        private async void BuildSelectedAddressablesForAllPlatforms()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            BuildAddressablesForSelectedPlatform();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            BuildAddressablesForSelectedPlatform();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            BuildAddressablesForSelectedPlatform();

            buildResult = CheckBuildResult();

            if (buildResult == EBuildError.None)
            {
                Debug.Log("Addressables built successfully for all selected platforms.");

                ZipBuildedScenes();

                await UploadBuildedScenes();
            }
            else
            {
                Debug.LogError("There were errors during the Addressables build process. Please check the details above.");
            }
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

        private void ZipBuildedScenes()
        {
            List<string> builtScenes = sceneConfigurations.SceneConfigurations
                .Where(x => x.IncludeInBuild)
                .Select(x => x.SceneNameFiltered)
                .ToList();

            foreach (string scene in builtScenes)
            {
                string fullBuildPath = Path.Combine(addressables_output_folder, scene);
                string fullZipPath = fullBuildPath + ".zip";

                if (!Directory.Exists(fullBuildPath))
                    throw new DirectoryNotFoundException($"Cartella build non trovata: {fullBuildPath}");

                if (File.Exists(fullZipPath))
                    File.Delete(fullZipPath);

                ZipFile.CreateFromDirectory(fullBuildPath, fullZipPath, System.IO.Compression.CompressionLevel.Optimal, true);
            }

        }


        // Dimensione massima di un blocco (in byte) per upload segmentato — 4 MB qui è una scelta sicura
        private const int ChunkSize = 4 * 1024 * 1024;

        private async Task UploadBuildedScenes()
        {
            List<string> builtScenes = sceneConfigurations.SceneConfigurations
                .Where(x => x.IncludeInBuild)
                .Select(x => x.SceneNameFiltered)
                .ToList();

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("Token non disponibile per l’upload su Azure. Interrompo l’operazione.");
                return;
            }

            string sasUrl = await GetSasUrl(token);

            foreach (string scene in builtScenes)
            {
                Debug.Log($"Inizio upload scena {scene}...");
                string sceneZipPath = scene + ".zip";
                await UploadZip(Path.Combine(addressables_output_folder, sceneZipPath), sasUrl);
                Debug.Log($"Upload scena {scene} completato.");
                Debug.Log($"Start import scene {scene} in Reflectis platform...");
                await ImportScene(token, scene);
                Debug.Log($"Scene {scene} imported.");
            }
        }


        public static async Task<string> GetSasUrl(string accessToken)
        {
            Debug.Log("==> Chiamata API protetta per ottenere SAS URL...");
            // URL della tua API protetta
            string apiUrl = "https://reflectis2023-api-prep.anothereality.io/worlds/25?api-version=2";

            // Imposta l’header di autorizzazione
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();

            World world = Newtonsoft.Json.JsonConvert.DeserializeObject<World>(json);
            httpClient = new HttpClient();
            Debug.Log($"SAS URL ricevuto: {world.UploadLink}");
            return world.UploadLink;
        }

        public class World
        {
            [SerializeField] private string uploadLink;

            public string UploadLink { get => uploadLink; set => uploadLink = value; }
        }

        public static async Task ImportScene(string accessToken, string zipName)
        {
            // URL della tua API protetta
            string apiUrl = "https://reflectis2023-api-prep.anothereality.io/worlds/25/environments/archives/import?api-version=2";

            // Imposta l’header di autorizzazione
            httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent($"\"{zipName + ".zip"}\"", Encoding.UTF8, "application/json");

            // Invia la richiesta POST con il body
            var response = await httpClient.PostAsync(apiUrl, content);

            // Controlla la risposta
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            else
            {
                Debug.Log($"Import {zipName} avviato con successo {JsonConvert.SerializeObject(response)}.");
            }
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Carica un file ZIP su Azure Blob Storage sotto una cartella specifica.
        /// Elimina eventuali blob esistenti nella cartella progetto.
        /// </summary>
        /// <summary>
        /// Carica un file ZIP su Azure Blob Storage sotto una cartella specifica,
        /// usando un SAS URL completo senza necessità di list o cancellazione.
        /// </summary>
        public async Task UploadZip(string filePath, string sasUrl)
        {
            string fileName = Path.GetFileName(filePath);

            // Usa UriBuilder per non rompere il SAS token
            var uriBuilder = new UriBuilder(sasUrl);
            uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + "/" + fileName;
            Uri uploadUri = uriBuilder.Uri;

            byte[] fileBytes = File.ReadAllBytes(filePath);
            var content = new ByteArrayContent(fileBytes);

            // Header obbligatorio per Blob Storage
            content.Headers.Add("x-ms-blob-type", "BlockBlob");

            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(uploadUri, content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log($"Upload completato: {fileName}");
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"Upload fallito: {fileName} SAS {uploadUri} - {response.StatusCode}\n{responseBody}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Errore upload {fileName}: {ex.Message}");
            }
        }


        #endregion

    }
}
