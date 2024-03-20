using Sirenix.Utilities;

using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class AddressablesConfigurationWindow : EditorWindow
    {
        private AddressableAssetSettings settings;

        private const string addressables_output_folder = "ServerData";
        private const string build_target = "[BuildTarget]";

        private string remoteBuildPath;
        private string remoteLoadPath;

        private string playerVersionOverride;

        [MenuItem("Reflectis/Addressables settings window")]
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

        private void LoadProfileSettings()
        {
            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);

            remoteBuildPath = string.Join('/', addressables_output_folder, build_target);

            var addressablesVariables = typeof(AddressablesVariables).GetProperties();
            remoteLoadPath = string.Join('/',
                RuntimeVariable($"{typeof(AddressablesVariables)}.{addressablesVariables[1].Name}"),
                RuntimeVariable($"{typeof(AddressablesVariables)}.{addressablesVariables[0].Name}"),
                build_target);

            playerVersionOverride = settings.OverridePlayerVersion;
        }

        private void DisplayAddressablesSettings()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true
            };

            // General settings
            EditorGUILayout.LabelField($"<b>Addressables general settings</b>", style);

            string activeProfileName = settings.profileSettings.GetProfileName(settings.activeProfileId);
            EditorGUILayout.LabelField($"Active addressables profile: <b>{activeProfileName}</b>", style);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{(IsPlayerVersionOverrideValid() ? "<b>[√]</b>" : "<b>[X]</b>")}", style, GUILayout.Width(20));
            EditorGUILayout.LabelField($"Catalog name: ", style, GUILayout.Width(100));
            playerVersionOverride = EditorGUILayout.TextArea(playerVersionOverride);
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(playerVersionOverride))
            {
                EditorGUILayout.LabelField($"Catalog name cannot be null!");
            }
            else if (!Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$"))
            {
                EditorGUILayout.LabelField($"Catalog name can contain only alphanumeric values!");
            }
            else if (settings.OverridePlayerVersion != playerVersionOverride)
            {
                settings.OverridePlayerVersion = playerVersionOverride;
            }

            EditorGUILayout.HelpBox("Catalog names should be univoque within the same world. If a new catalog is loaded through the Back office " +
                "with the same name of another one, the previous one is overridden.", MessageType.Info);

            EditorGUILayout.Space();

            if (!settings.BuildRemoteCatalog)
            {
                if (GUILayout.Button("Configure addressables settings", GUILayout.Width(250)))
                {
                    settings.BuildRemoteCatalog = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Addressables settings are properly configured!");
            }

            EditorGUILayout.Space();
            Rect rect1 = EditorGUILayout.GetControlRect(false, 1);
            rect1.height = 1;
            EditorGUI.DrawRect(rect1, new Color(0.5f, 0.5f, 0.5f, 1));

            // Profile settings
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"<b>Addressables profiles settings</b>", style);

            string remoteBuildPath = settings.profileSettings.GetValueByName(settings.activeProfileId, "Remote.BuildPath");
            bool isRemoteBuildPathConfigured = remoteBuildPath == this.remoteBuildPath;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{(isRemoteBuildPathConfigured ? "<b>[√]</b>" : "<b>[X]</b>")}", style, GUILayout.Width(20));
            EditorGUILayout.LabelField($"<b>RemoteBuildPath: </b>{remoteBuildPath}", style);
            EditorGUILayout.EndHorizontal();

            string remoteLoadPath = settings.profileSettings.GetValueByName(settings.activeProfileId, "Remote.LoadPath");
            bool isRemoteLoadPathConfigured = remoteLoadPath == this.remoteLoadPath;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{(isRemoteLoadPathConfigured ? "<b>[√]</b>" : "<b>[X]</b>")}", style, GUILayout.Width(20));
            EditorGUILayout.LabelField($"<b>RemoteLoadPath: </b>{remoteLoadPath}", style);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (!IsDefaultProfileConfigured())
            {
                if (GUILayout.Button("Configure remote build and load paths", GUILayout.Width(250)))
                {
                    ConfigureDefaultProfile();
                    DisplayAddressablesSettings();
                }
            }
            else
            {
                EditorGUILayout.LabelField("The addressables profile is properly configured!");
            }

            EditorGUILayout.Space();
            Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
            rect2.height = 1;
            EditorGUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 1));


            // Groups settings

            EditorGUILayout.LabelField("<b>Addressables Group settings</b>", style);

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

        private bool IsDefaultProfileConfigured()
        {
            return settings.profileSettings.GetValueByName(settings.activeProfileId, "Remote.BuildPath") == remoteBuildPath
                && settings.profileSettings.GetValueByName(settings.activeProfileId, "Remote.LoadPath") == remoteLoadPath;
        }

        private bool IsPlayerVersionOverrideValid()
        {
            return !string.IsNullOrEmpty(playerVersionOverride) && Regex.IsMatch(playerVersionOverride, @"[a-zA-Z][a-zA-Z0-9]*$");
        }

        private void ConfigureDefaultProfile()
        {
            settings.profileSettings.SetValue(settings.activeProfileId, "Remote.BuildPath", remoteBuildPath);
            settings.profileSettings.SetValue(settings.activeProfileId, "Remote.LoadPath", remoteLoadPath);

            EditorApplication.ExecuteMenuItem("File/Save Project");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string RuntimeVariable(string variable) => "{" + variable + "}";
    }
}