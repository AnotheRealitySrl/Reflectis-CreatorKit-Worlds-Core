using Sirenix.Utilities;

using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;
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

        private string remoteBuildPath;
        private string remoteLoadPath;

        private string playerVersionOverride;

        #endregion

        #region Unity callbacks

        [MenuItem("Reflectis/Configure and build Addressables")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(AddressablesConfigurationWindow));
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                LoadProfileSettings();
            }
            else
            {
                DisplayAddressablesSettings();
            }
        }

        #endregion

        #region Private methods

        private void LoadProfileSettings()
        {
            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);
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

        private void DisplayAddressablesSettings()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true,
            };

            #region Top-level settings

            EditorGUILayout.LabelField($"<b>General settings</b>", style);

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
                EditorGUILayout.LabelField($"<color=red>Catalog name cannot be null!</color>", style);
            }
            else if (!Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$"))
            {
                EditorGUILayout.LabelField($"<color=red>Catalog name can contain only alphanumeric values!</color>", style);
            }
            else if (settings.OverridePlayerVersion != playerVersionOverride)
            {
                settings.OverridePlayerVersion = playerVersionOverride;
            }

            EditorGUILayout.HelpBox("Catalog names should be univoque within the same world. If a new catalog is loaded through the Back office " +
                "with the same name of another one, the previous one is overridden.", MessageType.Info);

            EditorGUILayout.Space();

            if (!IsAddressablesSettingsConfigured())
            {
                if (GUILayout.Button("Configure addressables settings", GUILayout.Width(250)))
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
                    settings.ShaderBundleNaming = ShaderBundleNaming.ProjectName;
                    settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Disabled;
                    settings.DisableVisibleSubAssetRepresentations = false;
                    settings.BuildRemoteCatalog = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("<color=lime>General settings are properly configured!</color>", style);
            }

            EditorGUILayout.Space();
            Rect rect1 = EditorGUILayout.GetControlRect(false, 1);
            rect1.height = 1;
            EditorGUI.DrawRect(rect1, new Color(0.5f, 0.5f, 0.5f, 1));

            #endregion

            #region Profiles settings

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"<b>Profiles settings</b>", style);

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

            if (!IsDefaultProfileConfigured())
            {
                if (GUILayout.Button("Configure remote build and load paths", GUILayout.Width(250)))
                {
                    ConfigureDefaultProfile();
                }
            }
            else
            {
                EditorGUILayout.LabelField("<color=lime>The addressables profile is properly configured!</color>", style);
            }

            EditorGUILayout.Space();
            Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
            rect2.height = 1;
            EditorGUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 1));

            #endregion

            #region Groups settings

            EditorGUILayout.LabelField("<b>Groups settings</b>", style);

            if (!IsAddressablesGroupsConfigured())
            {
                if (GUILayout.Button("Configure addressables groups", GUILayout.Width(250)))
                {
                    settings.groups.ForEach(group =>
                    {
                        group.Schemas.Where(schema => schema is BundledAssetGroupSchema).ForEach(schema =>
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
                }
            }
            else
            {
                EditorGUILayout.LabelField("<color=lime>The groups are properly configured!</color>", style);
            }

            #endregion

            EditorGUILayout.Space();
            Rect rect3 = EditorGUILayout.GetControlRect(false, 1);
            rect3.height = 1;
            EditorGUI.DrawRect(rect3, new Color(0.5f, 0.5f, 0.5f, 1));

            if (IsDefaultProfileConfigured() && IsAddressablesSettingsConfigured() && IsAddressablesGroupsConfigured())
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

        }

        private bool IsDefaultProfileConfigured()
        {
            return settings.profileSettings.GetValueByName(settings.activeProfileId, remote_build_path_variable_name) == remoteBuildPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, remote_load_path_variable_name) == remoteLoadPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, build_target_variable_name) == build_target_variable_value
                && settings.profileSettings.GetValueByName(settings.activeProfileId, player_version_override_variable_name) == player_version_override_variable_value;
        }

        private bool IsAddressablesSettingsConfigured()
        {
            return
                !settings.BundleLocalCatalog &&
                IsPlayerVersionOverrideValid() &&
                settings.BuildRemoteCatalog &&
                settings.CheckForContentUpdateRestrictionsOption
                        == CheckForContentUpdateRestrictionsOptions.ListUpdatedAssetsWithRestrictions &&
                settings.MaxConcurrentWebRequests == 3 &&
                settings.CatalogRequestsTimeout == 0 &&
                !settings.IgnoreUnsupportedFilesInBuild &&
                !settings.UniqueBundleIds &&
                settings.ContiguousBundles &&
                settings.NonRecursiveBuilding &&
                settings.ShaderBundleNaming == ShaderBundleNaming.ProjectName &&
                settings.MonoScriptBundleNaming == MonoScriptBundleNaming.Disabled &&
                !settings.DisableVisibleSubAssetRepresentations;
        }

        private bool IsPlayerVersionOverrideValid()
        {
            return !string.IsNullOrEmpty(playerVersionOverride) && Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$");
        }

        private bool IsAddressablesGroupsConfigured()
        {
            bool configured = false;

            settings.groups.ForEach(group =>
            {
                group.Schemas.Where(schema => schema is BundledAssetGroupSchema).ForEach(schema =>
                {
                    BundledAssetGroupSchema bundledAssetGroupSchema = schema as BundledAssetGroupSchema;

                    configured =
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

        private void ConfigureDefaultProfile()
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


            EditorApplication.ExecuteMenuItem("File/Save Project");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string BuildtimeVariable(string variable) => "[" + variable + "]";
        private string RuntimeVariable(string variable) => "{" + variable + "}";

        #endregion
    }
}