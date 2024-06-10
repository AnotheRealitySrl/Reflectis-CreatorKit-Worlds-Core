#if SETUP_WINDOW_INSTALLED
using Reflectis.SetupEditor;
using UnityEditor;
using UnityEngine;


namespace Reflectis.SDK.CreatorKitEditor
{
    [InitializeOnLoad]
    public static class SetupGUIWindowAdds
    {
        static SetupGUIWindowAdds()
        {
            SetupReflectisWindow.configurationEvents.AddListener(AddAddressableButton);
        }

        private static void AddAddressableButton()
        {
            if (GUILayout.Button(new GUIContent("Addressables Configuration Window", "Opens the addressables configuration window, useful to setup the addressables in order to load your scene online")))
            {
                AddressablesConfigurationWindow window = EditorWindow.GetWindow<AddressablesConfigurationWindow>(typeof(SetupReflectisWindow));
                window.Show();
            }

            if (GUILayout.Button(new GUIContent("Setup Visual Scripting Nodes", "Setup the visual scripting nodes in order to load all the reflectis custom ones")))
            {
                VisualScriptingSetupEditor.Setup();
            }

            if (GUILayout.Button(new GUIContent("Network Placeholders Management", "Setup all the ids for all the networked gameObjects. Remember to use this window when you want to setup the networked elements in the scene")))
            {
                NetworkPlaceholdersManagementWindow window = EditorWindow.GetWindow<NetworkPlaceholdersManagementWindow>(typeof(SetupReflectisWindow));
                window.Show();
            }
            GUILayout.Space(10);
        }
    }
}
#endif
