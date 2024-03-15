using System.IO;

using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class AddressablesConfigurationWindow : EditorWindow
    {
        private AddressableAssetSettings settings;

        private string remoteLoadPath = "{Reflectis.AddressablesVariables.BaseUrl}/{Reflectis.AddressablesVariables.WorldId}/[BuildPlatform]";

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
                if (IsDefaultProfileConfigured())
                {
                    GUILayout.TextField("Addressables profile already configured");
                }

                if (GUILayout.Button("Configure addressables profile"))
                {
                    ConfigureDefaultProfile();
                }
            }
        }

        private void LoadProfileSettings()
        {
            settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);

            //remoteLoadPath = Path.Combine(nameof(AddressablesVariables.BaseUrl), nameof(AddressablesVariables.WorldId), "[BuildTarget]");
            remoteLoadPath = Path.Combine("{Reflectis.AddressablesVariables.BaseUrl}", "{Reflectis.AddressablesVariables.WorldId}", "[BuildTarget]");
        }

        private bool IsDefaultProfileConfigured()
        {
            return settings.profileSettings.GetValueById(settings.activeProfileId, "Remote.LoadPath") == remoteLoadPath;
        }

        private void ConfigureDefaultProfile()
        {
            settings.profileSettings.SetValue(settings.activeProfileId, "Remote.LoadPath", remoteLoadPath);

            EditorApplication.ExecuteMenuItem("File/Save Project");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}