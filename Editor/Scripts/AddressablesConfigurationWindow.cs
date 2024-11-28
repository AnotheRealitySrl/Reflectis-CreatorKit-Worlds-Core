using System;
using System.Collections.Generic;
using System.IO;
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
        private const string player_version_override_variable_value = "[Reflectis.SDK.CreatorKitEditor.AddressablesBuildScript.PlayerVersionOverride]";

        private const string environments_group_name = "Environments";
        [Obsolete("Thumbnails have to be uploaded from backoffice since Reflectis version 2024.9!")]
        private const string thumbnails_group_name = "Thumbnails";

        private string remoteBuildPath;
        private string remoteLoadPath;

        private string playerVersionOverride;

        private GUIStyle _toolbarButtonStyle;
        private Vector2 scrollPosition = Vector2.zero;

        private bool chooseOthersFoldout = false;

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

                //playerVersionOverride = EditorGUILayout.TextField(playerVersionOverride);
                GUI.enabled = false;
                if (playerVersionOverride != settings.OverridePlayerVersion)
                {
                    playerVersionOverride = settings.OverridePlayerVersion;
                }
                EditorGUILayout.TextField(playerVersionOverride);
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                if (string.IsNullOrEmpty(playerVersionOverride))
                {
                    EditorGUILayout.LabelField($"<color=red>The catalog name can not be null!</color>", style);
                }
                else if (!Regex.IsMatch(playerVersionOverride, alphanumeric_string_pattern))
                {
                    EditorGUILayout.LabelField($"<color=red>Only alphanumeric values are allowed!</color>", style);
                }

                //if (settings.OverridePlayerVersion != playerVersionOverride)
                //{
                //    UpdatePlayerVersion();
                //}

                ShowAddressablesBundles();

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

                EditorGUILayout.HelpBox($"Every addressable scene must be put inside the {environments_group_name} group." +
                        //$", " +
                        //    $"and the associated thumbnail inside the {thumbnails_group_name} group. " +
                        $"Note that each addressable asset must have a lower-case, alphanumeric name.",
                        MessageType.Info);

                if (!IsGroupValid(environments_group_name) /*|| !IsGroupValid(thumbnails_group_name)*/)
                {
                    EditorGUILayout.HelpBox($"Could not find one or more required addressables groups. Ckick on the button to fix the issue.",
                        MessageType.Error);

                    if (GUILayout.Button("Create missing groups", GUILayout.ExpandWidth(false)))
                    {
                        if (!IsGroupValid(environments_group_name))
                        {
                            CreateGroup(environments_group_name);
                        }
                        //if (!IsGroupValid(thumbnails_group_name))
                        //{
                        //    CreateGroup(thumbnails_group_name);
                        //}
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
                            bool isEntryNameValid = Regex.IsMatch(entry.address, alphanumeric_lowercase_string_pattern);
                            if (!isEntryNameValid)
                            {
                                if (GUILayout.Button("Fix", GUILayout.ExpandWidth(false)))
                                {
                                    UpdateAddressableEntry(entry);
                                }
                            }
                            EditorGUILayout.LabelField($"" +
                                $"{(isEntryNameValid ? (group.name == thumbnails_group_name ? "<b>[<color=yellow>obsolete</color>]</b>" : "<b>[<color=lime>√</color>]</b>") : "<b>[<color=red>X</color>]</b>")}" +
                                $" {entry}", style);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                //if (!IsAddressablesEntriesValid())
                //{
                //    EditorGUILayout.HelpBox($"There are inconsistencies between the {environments_group_name} and the {thumbnails_group_name} asset groups. " +
                //        $"Check if each environment has a corresponding thumbnail and viceversa, and there are not duplicate names within each group",
                //        MessageType.Error);
                //}

                EditorGUILayout.Space();

                if (!IsAddressablesGroupsConfigured())
                {
                    if (GUILayout.Button("Configure addressables groups", GUILayout.Width(250)))
                    {
                        ConfigureAddressablesGroups();
                    }
                }
                //else if (IsAddressablesEntriesValid())
                //{
                //    EditorGUILayout.LabelField("<color=lime>The groups settings are properly configured!</color>", style);
                //}

                EditorGUILayout.Space();

                #endregion

                CreateSeparator();
                EditorGUILayout.Space();

                if (IsAddressablesSettingsConfigured() && IsPlayerVersionOverrideValid() && IsProfileConfigured() && IsAddressablesGroupsConfigured() /*&& IsAddressablesEntriesValid()*/)
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

        private void ShowAddressablesBundles()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true,
            };

            // Get Addressables Bundles SO
            List<AddressablesBundleScriptableObject> addressablesBundleScriptableObjects = new List<AddressablesBundleScriptableObject>();
            List<string> addressablesBundleScriptableObjectsStr = AssetDatabase.FindAssets("t:" + typeof(AddressablesBundleScriptableObject).Name).ToList();
            foreach (string str in addressablesBundleScriptableObjectsStr)
            {
                string path = AssetDatabase.GUIDToAssetPath(str);
                addressablesBundleScriptableObjects.Add(AssetDatabase.LoadAssetAtPath<AddressablesBundleScriptableObject>(path));
            }

            // Addressables Bundle detail.
            // If found, then show the button to focus it, otherwise list all buttons and ask user to configure one of them.

            AddressablesBundleScriptableObject currentSO = null;
            foreach (var item in addressablesBundleScriptableObjects)
            {
                if (!string.IsNullOrEmpty(item.BundleName) && item.BundleName == playerVersionOverride)
                {
                    currentSO = item;
                    break;
                }
            }

            float bundleBtnWidth = (EditorGUIUtility.currentViewWidth - 30f) / 2f;
            float bundleNewBtnWidth = (EditorGUIUtility.currentViewWidth - 30f) / 3f;
            int btnIndex = -1;

            if (currentSO != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Bundle Asset: ", style, GUILayout.Width(100));
                if (GUILayout.Button(currentSO.name, GUILayout.Width(bundleBtnWidth)))
                {
                    Selection.activeObject = currentSO;
                    EditorGUIUtility.PingObject(currentSO);
                }
                EditorGUILayout.EndHorizontal();

                chooseOthersFoldout = EditorGUILayout.Foldout(chooseOthersFoldout, "Choose others");
                if (chooseOthersFoldout)
                {
                    EditorGUI.indentLevel++;
                    if (addressablesBundleScriptableObjects.Count > 1) // Do it only if 2+ configurations
                    {
                        btnIndex = -1;
                        foreach (var item in addressablesBundleScriptableObjects)
                        {
                            if (item.name != currentSO.name)
                            {
                                if (++btnIndex % 2f == 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                }
                                if (GUILayout.Button(item.name, GUILayout.Width(bundleBtnWidth)))
                                {
                                    Selection.activeObject = item;
                                    EditorGUIUtility.PingObject(item);
                                }
                                if (btnIndex % 2f == 1)
                                {
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                        if (btnIndex % 2f == 0) // Compensate
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (GUILayout.Button("[Create new]", GUILayout.Width(bundleNewBtnWidth)))
                    {
                        CreateNewAddressablesBundle();
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else if (addressablesBundleScriptableObjects.Count > 0)
            {
                EditorGUILayout.LabelField($"Choose a bundle and configure it from its Inspector.", style);
                btnIndex = -1;
                foreach (var item in addressablesBundleScriptableObjects)
                {
                    if (++btnIndex % 2f == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    if (GUILayout.Button(item.name, GUILayout.Width(bundleBtnWidth)))
                    {
                        Selection.activeObject = item;
                        EditorGUIUtility.PingObject(item);
                    }
                    if (btnIndex % 2f == 1)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                if (btnIndex % 2f == 0) // Compensate
                {
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("[Create new]", GUILayout.Width(bundleNewBtnWidth)))
                {
                    CreateNewAddressablesBundle();
                }
            }
            else
            {
                EditorGUILayout.LabelField($"You need to create an Addressable Bundle to proceed!", style);
                if (GUILayout.Button("[Create new]", GUILayout.Width(bundleNewBtnWidth)))
                {
                    CreateNewAddressablesBundle();
                }
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
            return !string.IsNullOrEmpty(playerVersionOverride) && Regex.IsMatch(playerVersionOverride, alphanumeric_string_pattern);
        }

        private void UpdatePlayerVersion()
        {
            settings.OverridePlayerVersion = playerVersionOverride;
            settings.ShaderBundleCustomNaming = playerVersionOverride;

            SaveChanges();
        }

        private void CreateNewAddressablesBundle()
        {
            string newBundleName = "new" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string configFolder = "Assets/ReflectisSettings/Editor/AddressablesConfigurations";
            string configName = "AddressablesBundle-" + newBundleName;

            AddressablesBundleScriptableObject ab = null;
            var path = configFolder + "/" + configName + ".asset";
            ab = CreateInstance<AddressablesBundleScriptableObject>();

            // Maybe initialize something here...
            // (here)

            Directory.CreateDirectory(configFolder);
            AssetDatabase.CreateAsset(ab, path);
            ab = AssetDatabase.LoadAssetAtPath<AddressablesBundleScriptableObject>(path);

            Selection.activeObject = ab;
            EditorGUIUtility.PingObject(ab);

            AssetDatabase.SaveAssets();
        }

        private bool IsAddressablesSettingsConfigured()
        {
            return
                settings.RemoteCatalogLoadPath.GetName(settings) == remote_load_path_variable_name &&
                settings.RemoteCatalogBuildPath.GetName(settings) == remote_build_path_variable_name &&
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
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, remote_load_path_variable_name);
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, remote_build_path_variable_name);
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

            if (!IsGroupValid(environments_group_name) /*|| !IsGroupValid(thumbnails_group_name)*/)
            {
                return false;
            }

            settings.groups.ForEach(group =>
            {
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

        //Checks if there is an env with no thumbnail and viceversa
        //private bool IsAddressablesEntriesValid()
        //{
        //    if (settings.groups.Count == 0)
        //        return true;

        //    IEnumerable<string> environmentEntries = settings.groups.Find(x => x.Name == environments_group_name).entries.Select(x => x.address);
        //    IEnumerable<string> thumbnailEntries = settings.groups.Find(x => x.Name == thumbnails_group_name).entries.Select(x => x.address);

        //    return
        //        environmentEntries.All(new HashSet<string>().Add)
        //    &&
        //    thumbnailEntries.All(new HashSet<string>().Add) &&
        //    !environmentEntries.Except(thumbnailEntries).Any() &&
        //    !thumbnailEntries.Except(environmentEntries).Any();
        //}

        private void UpdateAddressableEntry(AddressableAssetEntry entry)
        {
            entry.SetAddress(Regex.Replace(entry.address.ToLower(), alphanumeric_lowercase_string_pattern_negated, string.Empty));

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
    }
}