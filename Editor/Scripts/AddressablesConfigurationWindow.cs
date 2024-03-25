using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class AddressablesConfigurationWindow : EditorWindow
    {
        #region Private variables

        private AddressableAssetSettings settings;

        private const string addressables_output_folder = "ServerData";

        private const string remote_build_path_variable_name = "Remote.BuildPath";
        private const string remote_load_path_variable_name = "Remote.LoadPath";

        private const string build_target_variable_name = "BuildTarget";
        private const string build_target_variable_value = "[UnityEditor.EditorUserBuildSettings.activeBuildTarget]";
        private const string player_version_override_variable_name = "PlayerVersionOverride";
        private const string player_version_override_variable_value = "[Reflectis.SDK.CreatorKitEditor.AddressablesBuildScript.PlayerVersionOverride]";

        private const string environments_group_name = "Environments";
        private const string thumbnails_group_name = "Thumbnails";

        private string remoteBuildPath;
        private string remoteLoadPath;

        private string playerVersionOverride;

        private GUIStyle _toolbarButtonStyle;
        private Vector2 scrollPosition = Vector2.zero;

        #endregion

        #region Unity callbacks

        [MenuItem("Reflectis/Configure and build Addressables")]
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

        private void OnGUI()
        {
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

                #region Top-level settings

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"<b>General settings</b>", style, GUILayout.Width(100));
                if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                {
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Settings");
                }
                EditorGUILayout.EndHorizontal();

                string activeProfileName = settings.profileSettings.GetProfileName(settings.activeProfileId);
                EditorGUILayout.LabelField($"Active addressables profile: <b>{activeProfileName}</b>", style);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{(IsPlayerVersionOverrideValid() ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                EditorGUILayout.LabelField($"Catalog name: ", style, GUILayout.Width(100));
                playerVersionOverride = EditorGUILayout.TextArea(playerVersionOverride);
                EditorGUILayout.EndHorizontal();

                if (string.IsNullOrEmpty(playerVersionOverride))
                {
                    EditorGUILayout.LabelField($"<color=red>The catalog name can not be null!</color>", style);
                }
                else if (!Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$"))
                {
                    EditorGUILayout.LabelField($"<color=red>Only alphanumeric values are allowed!</color>", style);
                }

                if (settings.OverridePlayerVersion != playerVersionOverride)
                {
                    settings.OverridePlayerVersion = playerVersionOverride;
                    settings.ShaderBundleCustomNaming = playerVersionOverride;
                }

                EditorGUILayout.HelpBox("Catalog names should be univoque within the same world. If a new catalog is loaded through the Back office " +
                    "with the same name of another one, the previous one is overridden.", MessageType.Info);

                EditorGUILayout.Space();

                if (!IsAddressablesSettingsConfigured())
                {
                    if (GUILayout.Button("Configure addressables settings", GUILayout.Width(250)))
                    {
                        ConfigureAddressablesSettings();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("<color=lime>General settings are properly configured!</color>", style);
                }

                EditorGUILayout.Space();
                CreateSeparator();

                #endregion

                #region Profiles settings

                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"<b>Profile settings</b>", style, GUILayout.Width(100));
                if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                {
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");
                }
                EditorGUILayout.EndHorizontal();

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

                EditorGUILayout.Space();

                if (!IsProfileConfigured())
                {
                    if (GUILayout.Button("Configure remote build and load paths", GUILayout.Width(250)))
                    {
                        ConfigureProfile();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("<color=lime>The addressables profile is properly configured!</color>", style);
                }

                EditorGUILayout.Space();
                CreateSeparator();

                #endregion

                #region Groups settings

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<b>Groups settings</b>", style, GUILayout.Width(100));
                if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                {
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox($"Every addressable scene must be put inside the {environments_group_name} group, " +
                        $"and the associated thumbnail inside the {thumbnails_group_name} group. " +
                        $"Note that each addressable asset must have a lower-case, alphanumeric name.",
                        MessageType.Info);

                if (!IsGroupValid(environments_group_name) || !IsGroupValid(thumbnails_group_name))
                {
                    EditorGUILayout.HelpBox($"Could not find one or more required addressables groups. Ckick on the button to fix the issue.",
                        MessageType.Error);

                    if (GUILayout.Button("Create missing groups", GUILayout.ExpandWidth(false)))
                    {
                        if (!IsGroupValid(environments_group_name))
                        {
                            CreateGroup(environments_group_name);
                        }
                        if (!IsGroupValid(thumbnails_group_name))
                        {
                            CreateGroup(thumbnails_group_name);
                        }
                        ConfigureAddressablesGroups();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    foreach (var group in settings.groups.Where(x => !x.SchemaTypes.Contains(typeof(PlayerDataGroupSchema)) && x != settings.DefaultGroup))
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField($"<b>{group.name}</b>", style);
                        foreach (var entry in group.entries.OrderBy(x => x.address))
                        {
                            EditorGUILayout.BeginHorizontal();
                            bool isEntryNameValid = new Regex(@"^[a-z0-9\s,]*$").IsMatch(entry.address);
                            if (!isEntryNameValid)
                            {
                                if (GUILayout.Button("Fix", GUILayout.ExpandWidth(false)))
                                {
                                    entry.address = Regex.Replace(entry.address.ToLower(), "[^a-z0-9 -]", string.Empty);
                                }
                            }
                            EditorGUILayout.LabelField($"{(isEntryNameValid ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")} {entry}", style);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (!IsAddressablesEntriesValid())
                {
                    EditorGUILayout.HelpBox($"There are inconsistencies between the {environments_group_name} and the {thumbnails_group_name} asset groups. " +
                        $"Check if each environment has a corresponding thumbnail and viceversa, and there are not duplicate names within each group",
                        MessageType.Error);
                }

                EditorGUILayout.Space();

                if (!IsAddressablesGroupsConfigured())
                {
                    if (GUILayout.Button("Configure addressables groups", GUILayout.Width(250)))
                    {
                        ConfigureAddressablesGroups();
                    }
                }
                else if (IsAddressablesEntriesValid())
                {
                    EditorGUILayout.LabelField("<color=lime>The groups settings are properly configured!</color>", style);
                }

                EditorGUILayout.Space();

                #endregion

                CreateSeparator();
                EditorGUILayout.Space();

                if (IsAddressablesSettingsConfigured() && IsPlayerVersionOverrideValid() && IsProfileConfigured() && IsAddressablesGroupsConfigured() && IsAddressablesEntriesValid())
                {
                    if (GUILayout.Button("Build Addressables", EditorStyles.miniButtonMid))
                    {
                        AddressablesBuildScript.BuildAddressablesForAllPlatforms();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("<color=red>There are some configuration issue in the addressables. Please fix them before building.</color>", style);
                }

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

        private bool IsPlayerVersionOverrideValid()
        {
            return !string.IsNullOrEmpty(playerVersionOverride) && Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$");
        }

        private bool IsAddressablesSettingsConfigured()
        {
            return
                !settings.BundleLocalCatalog &&
                settings.BuildRemoteCatalog &&
                settings.CheckForContentUpdateRestrictionsOption
                        == CheckForContentUpdateRestrictionsOptions.ListUpdatedAssetsWithRestrictions &&
                settings.MaxConcurrentWebRequests == 3 &&
                settings.CatalogRequestsTimeout == 0 &&
                !settings.IgnoreUnsupportedFilesInBuild &&
                !settings.UniqueBundleIds &&
                settings.ContiguousBundles &&
                settings.NonRecursiveBuilding &&
                settings.ShaderBundleNaming == ShaderBundleNaming.Custom &&
                settings.ShaderBundleCustomNaming == playerVersionOverride &&
                settings.MonoScriptBundleNaming == MonoScriptBundleNaming.Disabled &&
                !settings.DisableVisibleSubAssetRepresentations;
        }

        private void ConfigureAddressablesSettings()
        {
            settings.BundleLocalCatalog = false;
            settings.BuildRemoteCatalog = true;
            settings.CheckForContentUpdateRestrictionsOption = CheckForContentUpdateRestrictionsOptions.ListUpdatedAssetsWithRestrictions;
            settings.ContentStateBuildPath = string.Empty;
            settings.MaxConcurrentWebRequests = 3;
            settings.CatalogRequestsTimeout = 0;
            settings.IgnoreUnsupportedFilesInBuild = false;
            settings.UniqueBundleIds = false;
            settings.ContiguousBundles = true;
            settings.NonRecursiveBuilding = true;
            settings.ShaderBundleNaming = ShaderBundleNaming.Custom;
            settings.ShaderBundleCustomNaming = playerVersionOverride;
            settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Disabled;
            settings.DisableVisibleSubAssetRepresentations = false;
            settings.BuildRemoteCatalog = true;

            SaveSettings();
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

            if (string.IsNullOrEmpty(settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name)))
            {
                settings.profileSettings.CreateValue(build_target_variable_name, build_target_variable_value);
            }
            else
            {
                settings.profileSettings.SetValue(settings.activeProfileId, build_target_variable_name, build_target_variable_value);
            }

            if (string.IsNullOrEmpty(settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name)))
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

        private bool IsGroupValid(string groupName)
        {
            return settings.groups.Find(x => x.Name == groupName);
        }

        private void CreateGroup(string groupName)
        {
            settings.CreateGroup(groupName, false, false, true, settings.DefaultGroup.Schemas);
        }

        private bool IsAddressablesGroupsConfigured()
        {
            bool configured = true;

            if (!IsGroupValid(environments_group_name) || !IsGroupValid(thumbnails_group_name))
            {
                return false;
            }

            settings.groups.ForEach(group =>
            {
                group.Schemas.Where(schema => schema is BundledAssetGroupSchema).ToList().ForEach(schema =>
                {
                    BundledAssetGroupSchema bundledAssetGroupSchema = schema as BundledAssetGroupSchema;

                    configured &=
                        bundledAssetGroupSchema.Compression == BundledAssetGroupSchema.BundleCompressionMode.LZ4 &&
                        bundledAssetGroupSchema.IncludeInBuild == true &&
                        bundledAssetGroupSchema.ForceUniqueProvider == false &&
                        bundledAssetGroupSchema.UseAssetBundleCache == true &&
                        bundledAssetGroupSchema.UseAssetBundleCrc == true &&
                        bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles == true &&
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
                        bundledAssetGroupSchema.AssetBundledCacheClearBehavior == BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache &&
                        bundledAssetGroupSchema.BundleMode == (group.IsDefaultGroup()
                            ? BundledAssetGroupSchema.BundlePackingMode.PackTogether
                            : BundledAssetGroupSchema.BundlePackingMode.PackSeparately) &&
                        bundledAssetGroupSchema.BundleNaming == BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                });
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

                    bundledAssetGroupSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                    bundledAssetGroupSchema.IncludeInBuild = true;
                    bundledAssetGroupSchema.ForceUniqueProvider = false;
                    bundledAssetGroupSchema.UseAssetBundleCache = true;
                    bundledAssetGroupSchema.UseAssetBundleCrc = true;
                    bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = true;
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
                    bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
                    bundledAssetGroupSchema.BundleMode = group.IsDefaultGroup()
                            ? BundledAssetGroupSchema.BundlePackingMode.PackTogether
                            : BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                    bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                });
            });

            SaveSettings();
        }

        private bool IsAddressablesEntriesValid()
        {
            if (settings.groups.Count == 0)
                return true;

            IEnumerable<string> environmentEntries = settings.groups.Find(x => x.Name == environments_group_name).entries.Select(x => x.address);
            IEnumerable<string> thumbnailEntries = settings.groups.Find(x => x.Name == thumbnails_group_name).entries.Select(x => x.address);

            return
                environmentEntries.All(new HashSet<string>().Add) &&
                thumbnailEntries.All(new HashSet<string>().Add) &&
                !environmentEntries.Except(thumbnailEntries).Any() &&
                !thumbnailEntries.Except(environmentEntries).Any();
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
    }
}