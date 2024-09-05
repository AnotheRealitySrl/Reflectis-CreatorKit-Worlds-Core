using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    #region Addressable Asset Classes

    [Serializable]
    public class EnvironmentAssetDefinition
    {
        [SerializeField]
        private string addressableAssetName = string.Empty;

        [SerializeField]
        private SceneAsset sceneAsset = null;

        [SerializeField]
        private Texture thumbnailAsset = null;

        public string AddressableAssetName => addressableAssetName;
        public SceneAsset SceneAsset => sceneAsset;
        public Texture ThumbnailAsset => thumbnailAsset;
    }
    
    /*
     * Freezed! Right not we are not compatible with custom addressable assets that are not managed as we do with localization and envs!
     * 
    
    [Serializable]
    public class CustomAddressableAssetDefinition
    {
        [SerializeField]
        private string addressableAssetName = string.Empty;

        [SerializeField]
        private UnityEngine.Object asset = null;

        public string AddressableAssetName => addressableAssetName;
        public UnityEngine.Object Asset => asset;
    }
    */

    #endregion

    [CreateAssetMenu(fileName = "AddressablesBundle", menuName = "Reflectis/SDK-CreatorKit/Editor-AddressablesBundle", order = 1)]
    public class AddressablesBundleScriptableObject : ScriptableObject
    {
        #region Consts

        internal const string environments_group_name = "Environments";
        internal const string thumbnails_group_name = "Thumbnails";

        #endregion

        #region Inspector Info

        [SerializeField]
        private string bundleName = string.Empty;

        [Space]

        [SerializeField]
        private TextAsset localizationAsset = null;

        [Space]

        /*
         * See note above!
         * 
        
        [SerializeField]
        private List<CustomAddressableAssetDefinition> customAssets = new List<CustomAddressableAssetDefinition>();
        */

        [SerializeField]
        private List<EnvironmentAssetDefinition> environmentAssets = new List<EnvironmentAssetDefinition>();

        #endregion

        #region Properties

        public string BundleName => bundleName;

        #endregion

        #region Execution: Setup

        internal void Setup()
        {
            // Warning: This method works well only if the Creator Kit and Reflectis Addressable Groups Setup has been done correctly!
            Debug.Log("Clicked setup for bundle \"" + bundleName + "\".");

            AddressableAssetSettings settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);
            if (settings)
            {
                settings.OverridePlayerVersion = bundleName;
                settings.ShaderBundleCustomNaming = bundleName;

                foreach (var group in settings.groups)
                {
                    if (group == settings.DefaultGroup)
                    {
                        var entriesAdded = new List<AddressableAssetEntry>();

                        string path, guid;

                        if (localizationAsset != null)
                        {
                            path = AssetDatabase.GetAssetPath(localizationAsset);
                            guid = AssetDatabase.AssetPathToGUID(path);

                            var localizationEntry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                            localizationEntry.address = bundleName + "-localizationcsv";
                            localizationEntry.labels.Clear();

                            entriesAdded.Add(localizationEntry);
                        }

                        /*
                         * See note above!
                         * 
                        
                        for (int i = 0; i < customAssets.Count; i++)
                        {
                            var asset = customAssets[i];
                            path = AssetDatabase.GetAssetPath(asset.Asset);
                            guid = AssetDatabase.AssetPathToGUID(path);

                            var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                            entry.address = asset.AddressableAssetName;
                            entry.labels.Clear();

                            entriesAdded.Add(entry);
                        }
                        */

                        foreach (AddressableAssetEntry entry in group.entries.Except(entriesAdded).ToArray())
                            group.RemoveAssetEntry(entry);

                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
                    }
                    else if (group.Name == environments_group_name)
                    {
                        var entriesAdded = new List<AddressableAssetEntry>();
                        for (int i = 0; i < environmentAssets.Count; i++)
                        {
                            var asset = environmentAssets[i];
                            var path = AssetDatabase.GetAssetPath(asset.SceneAsset);
                            var guid = AssetDatabase.AssetPathToGUID(path);

                            var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                            entry.address = asset.AddressableAssetName;
                            entry.labels.Clear();

                            entriesAdded.Add(entry);
                        }

                        foreach (AddressableAssetEntry entry in group.entries.Except(entriesAdded).ToArray())
                            group.RemoveAssetEntry(entry);

                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
                    }
                    else if (group.Name == thumbnails_group_name)
                    {
                        var entriesAdded = new List<AddressableAssetEntry>();
                        for (int i = 0; i < environmentAssets.Count; i++)
                        {
                            var asset = environmentAssets[i];
                            var path = AssetDatabase.GetAssetPath(asset.ThumbnailAsset);
                            var guid = AssetDatabase.AssetPathToGUID(path);

                            var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                            entry.address = asset.AddressableAssetName;
                            entry.labels.Clear();

                            entriesAdded.Add(entry);
                        }

                        foreach (AddressableAssetEntry entry in group.entries.Except(entriesAdded).ToArray())
                            group.RemoveAssetEntry(entry);

                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
                    }
                }
            }
        }

        #endregion
    }

    [CustomEditor(typeof(AddressablesBundleScriptableObject))]
    public class AddressablesBundleScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AddressablesBundleScriptableObject obj = (AddressablesBundleScriptableObject)target;

            GUILayout.Space(10);
            GUI.enabled = !Application.isPlaying;

            // Check if configuration is ok.
            bool isEverythingOk = false;

            AddressableAssetSettings settings = AddressablesBuildScript.GetSettingsObject(AddressablesBuildScript.settings_asset);
            if (settings)
            {
                var groups = settings.groups;
                if (groups.Exists(x => x.Name == AddressablesBundleScriptableObject.environments_group_name) && groups.Exists(x => x.Name == AddressablesBundleScriptableObject.thumbnails_group_name))
                {
                    // If the two mandatory addressable groups are set, then the configuration is ok.
                    isEverythingOk = true;
                }
            }

            if (isEverythingOk)
            {
                if (GUILayout.Button("Configure Addressables Bundle"))
                {
                    obj.Setup();
                }
            }
            else
            {
                if (GUILayout.Button($"Go to \"{AddressablesConfigurationWindow.window_name}\"!"))
                {
                    EditorWindow.GetWindow(typeof(AddressablesConfigurationWindow));
                }
            }

            GUI.enabled = true;
        }
    }
}