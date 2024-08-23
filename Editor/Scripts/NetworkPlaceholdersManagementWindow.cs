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

        private Vector2 scrollPosition = Vector2.zero;

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

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            {
                Dictionary<int, string> usedCodes = new Dictionary<int, string>();

                foreach (var placeholder in networkPlaceholders)
                {
                    int tmpCode = ((INetworkPlaceholder)placeholder).InitializationId;
                    bool tmpIsUsed = usedCodes.TryGetValue(tmpCode, out string alreadyUsedGOName);

                    string tmpCodeStr = tmpCode.ToString();
                    string tmpFeedback = string.Empty;

                    if (tmpIsUsed)
                    {
                        tmpCodeStr = "<color=red>" + tmpCodeStr + "</color>";
                        tmpFeedback = "Used by: <b>" + alreadyUsedGOName + "</b>";
                    }
                    else
                    {
                        usedCodes.Add(tmpCode, placeholder.gameObject.name);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField($"<b>{placeholder.gameObject.name}</b>", style, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f));

                        EditorGUILayout.LabelField($"{tmpCodeStr}", style, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));

                        EditorGUILayout.LabelField($"{tmpFeedback}", style, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
