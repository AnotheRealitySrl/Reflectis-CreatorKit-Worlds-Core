using Reflectis.SDK.CreatorKit;

using Sirenix.Utilities;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class NetworkPlaceholdersManagementWindow : EditorWindow
    {
        private List<SceneComponentPlaceholderBase> networkPlaceholders = new();
        private string networkPlaceholdersStringified;

        [MenuItem("Reflectis/Network placeholders management")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(NetworkPlaceholdersManagementWindow));
        }

        private void OnGUI()
        {
            FindNetworkPlaceholders();
            DisplayPlaceholders();


            if (GUILayout.Button("Initialize network placeholders IDs"))
            {
                FindNetworkPlaceholders();
                SetPlaceholdersNewIDs();
                DisplayPlaceholders();
            }

            GUILayoutOption[] options = new GUILayoutOption[] {
                GUILayout.ExpandHeight(true)};
            EditorGUILayout.LabelField(new GUIContent(networkPlaceholdersStringified), options);
        }

        private void FindNetworkPlaceholders()
        {
            Scene s = SceneManager.GetActiveScene();
            GameObject[] gameObjects = s.GetRootGameObjects();

            networkPlaceholders.Clear();
            gameObjects.ForEach(x => networkPlaceholders.AddRange(x.GetComponentsInChildren<SceneComponentPlaceholderBase>(true)
                    .Where(p => p is INetworkPlaceholder np && np.IsNetworked)));
        }

        private void SetPlaceholdersNewIDs()
        {
            networkPlaceholders.ForEach(p =>
            {
                ((INetworkPlaceholder)p).InitializationId = networkPlaceholders.IndexOf(p) + 1;
                EditorUtility.SetDirty(p);
            });
        }

        private void DisplayPlaceholders()
        {
            networkPlaceholdersStringified = string.Empty;
            foreach (var placeholder in networkPlaceholders)
            {
                networkPlaceholdersStringified += $"{placeholder.gameObject.name}: {((INetworkPlaceholder)placeholder).InitializationId} \n";
            }
        }
    }
}
