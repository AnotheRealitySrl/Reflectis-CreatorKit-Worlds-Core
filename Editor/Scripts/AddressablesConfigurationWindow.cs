using Reflectis.CreatorKit.Worlds.Core.Editor;

using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

using UnityEngine;

using static Reflectis.CreatorKit.Worlds.Core.Editor.SceneListScriptableObject;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{
    public class AddressablesConfigurationWindow : EditorWindow
    {
        #region Public consts

        public const string window_name = "Configure and build Addressables";

        #endregion

        #region Private variables

        private const string alphanumeric_string_pattern = @"^[a-zA-Z0-9]*$";
        private const string alphanumeric_lowercase_string_pattern = @"^[a-z0-9]*$";
        private const string alphanumeric_lowercase_string_pattern_negated = @"[^a-z0-9]";

        private AddressableAssetSettings settings;

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

        private GUIStyle _toolbarButtonStyle;
        private Vector2 scrollPosition = Vector2.zero;

        private SceneListScriptableObject sceneConfigurations;

        private bool addressablesSettingsFoldout;
        private bool topLevelSettingsFoldout;
        private bool profileSettingsFoldout;
        private bool groupsSettingsFoldout;


        #endregion

        #region Unity callbacks

        [MenuItem("Reflectis/" + window_name)]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(AddressablesConfigurationWindow));
        }

        private void Awake()
        {
            if (settings == null)
            {
                LoadProfileSettings();
            }
        }

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);

        private void OnGUI()
        {
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            DisplayAddressablesSettings();
        }

        #endregion

        #region Private methods

        private void LoadProfileSettings()
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

        /// <summary>
        /// Draw buttons on toolbar.
        /// Automatically called by unity.
        /// </summary>
        /// <param name="position"></param>
        private void ShowButton(Rect position)
        {
            _toolbarButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset()
            };

            if (GUI.Button(position, EditorGUIUtility.IconContent("_Help", "Doc|Open documentation"), _toolbarButtonStyle))
            {
                Application.OpenURL("https://reflectis.io/docs/CK/gettingstarted/startanewproject/Addressable-setup/");
            }
        }

        private void DisplayAddressablesSettings()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true,
            };
            GUIStyle dropdownStyle = new(EditorStyles.foldoutHeader)
            {
                richText = true,
            };

            if (!settings)
            {
                EditorGUILayout.HelpBox("No Addressables settings found! Click on \"Create Addressable settings\" to create them",
                  MessageType.Warning);

                if (GUILayout.Button("Create Addressables settings"))
                {
                    AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                        AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);

                    LoadProfileSettings();
                }
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

                #region Scene Configuration

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
                        EditorGUILayout.PropertyField(property, true);
                    }

                    serializedObject.ApplyModifiedProperties();
                }
                else
                {

                }

                EditorGUILayout.Space();

                if (IsAddressablesSettingsConfigured() && IsProfileConfigured() && AreAddressablesGroupsConfigured() /*&& IsAddressablesEntriesValid()*/)
                {
                    if (GUILayout.Button("Build Addressables", EditorStyles.miniButtonMid))
                    {
                        BuildSelectedAddressablesForAllPlatforms();
                    }
                }
                else
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField("<color=red>There are some configuration issue in the addressables. Please fix them before building.</color>", style);

                    if (GUILayout.Button("Fix addressables configuration", EditorStyles.miniButtonMid))
                    {
                        ConfigureAddressablesSettings();
                        ConfigureProfile();
                        ConfigureAddressablesGroups();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                CreateSeparator();
                EditorGUILayout.Space();

                #endregion

                #region Addressables settings configuration

                addressablesSettingsFoldout = EditorGUILayout.Foldout(addressablesSettingsFoldout, $" Show advanced addressables configurations", true, dropdownStyle);

                if (addressablesSettingsFoldout)
                {
                    #region Top-level settings

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    topLevelSettingsFoldout = EditorGUILayout.Foldout(topLevelSettingsFoldout, $"{(IsAddressablesSettingsConfigured() ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")} Top-level settings", true, dropdownStyle);
                    if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Settings");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (topLevelSettingsFoldout)
                    {
                        EditorGUILayout.BeginVertical(new GUIStyle { margin = new RectOffset(20, 0, 0, 0) });

                        string activeProfileName = settings.profileSettings.GetProfileName(settings.activeProfileId);
                        EditorGUILayout.LabelField($"Active addressables profile: <b>{activeProfileName}</b>", style);

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    #endregion

                    #region Profiles settings

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    profileSettingsFoldout = EditorGUILayout.Foldout(profileSettingsFoldout, $"{(IsProfileConfigured() ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")} Profile settings", true, dropdownStyle);
                    if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (profileSettingsFoldout)
                    {
                        EditorGUILayout.BeginVertical(new GUIStyle { margin = new RectOffset(20, 0, 0, 0) });

                        string remoteBuildPath = settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name);
                        bool isRemoteBuildPathConfigured = remoteBuildPath == this.remoteBuildPath;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{(isRemoteBuildPathConfigured ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                        EditorGUILayout.LabelField($"<b>{remote_build_path_variable_name}: </b>{remoteBuildPath}", style);
                        EditorGUILayout.EndHorizontal();

                        string remoteLoadPath = settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name);
                        bool isRemoteLoadPathConfigured = remoteLoadPath == this.remoteLoadPath;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{(isRemoteLoadPathConfigured ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                        EditorGUILayout.LabelField($"<b>{remote_load_path_variable_name}: </b>{remoteLoadPath}", style);
                        EditorGUILayout.EndHorizontal();

                        string buildTarget = settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name);
                        bool isBuildTargetConfigured = buildTarget == build_target_variable_value;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{(isBuildTargetConfigured ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                        EditorGUILayout.LabelField($"<b>{build_target_variable_name}: </b>{buildTarget}", style);
                        EditorGUILayout.EndHorizontal();

                        string playerVersionOverrideVariable = settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name);
                        bool isPlayerVersionOverrideConfiugred = playerVersionOverrideVariable == player_version_override_variable_value;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{(isPlayerVersionOverrideConfiugred ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                        EditorGUILayout.LabelField($"<b>{player_version_override_variable_name}: </b>{playerVersionOverrideVariable}", style);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    #endregion

                    #region Groups settings

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    groupsSettingsFoldout = EditorGUILayout.Foldout(groupsSettingsFoldout, $"{(AreAddressablesGroupsConfigured() ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")} Groups settings", true, dropdownStyle);
                    if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (groupsSettingsFoldout)
                    {
                        EditorGUILayout.BeginVertical(new GUIStyle { margin = new RectOffset(20, 0, 0, 0) });

                        foreach (var group in settings.groups)
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

                            EditorGUILayout.LabelField($"{(IsAddresableGroupConfigured(group) ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")} {group.Name}", style);
                            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                            {
                                Selection.activeObject = group;
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    #endregion
                }

                #endregion

                GUILayout.EndScrollView();
            }
        }

        private void CreateSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        #region Top-Level settings configuration

        private bool IsAddressablesSettingsConfigured()
        {
            return
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
        }

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

        private bool IsProfileConfigured()
        {
            return settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name) == remoteBuildPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name) == remoteLoadPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name) == build_target_variable_value
                && settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name) == player_version_override_variable_value;
        }

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

        private bool AreAddressablesGroupsConfigured()
        {
            bool configured = true;
            settings.groups.ForEach(group => configured &= IsAddresableGroupConfigured(group));
            return configured;
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

        #endregion


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