using Reflectis.SDK.CreatorKit;

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
                SetPlaceholdersNewIDs();
            }
        }

        private void FindNetworkPlaceholders()
        {
            Scene s = SceneManager.GetActiveScene();
            GameObject[] gameObjects = s.GetRootGameObjects();

            networkPlaceholders.Clear();

            foreach (GameObject obj in gameObjects)
            {
                networkPlaceholders.AddRange(obj.GetComponentsInChildren<SceneComponentPlaceholderBase>(true)
                        .Where(p => p is INetworkPlaceholder np && np.IsNetworked));
            }
        }

        private void SetPlaceholdersNewIDs()
        {
            System.Random rnd = new();
            networkPlaceholders.ForEach(p =>
            {
                ((INetworkPlaceholder)p).InitializationId = rnd.Next(1, 99999);
                EditorUtility.SetDirty(p);
            });
        }

        private void DisplayPlaceholders()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true
            };

            EditorGUILayout.BeginScrollView(Vector2.zero);
            EditorGUILayout.BeginVertical();

            foreach (var placeholder in networkPlaceholders)
            {
                EditorGUILayout.LabelField($"<b>{placeholder.gameObject.name}</b>: {((INetworkPlaceholder)placeholder).InitializationId}", style);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
