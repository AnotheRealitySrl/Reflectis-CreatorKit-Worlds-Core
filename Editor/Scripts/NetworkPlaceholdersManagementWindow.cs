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

            GUIStyle btnStyle = new(GUI.skin.button)
            {
                richText = true
            };

            if (GUILayout.Button("Initialize invalid network placeholders IDs", btnStyle))
            {
                SetPlaceholdersNewIDs();
            }
            if (GUILayout.Button("Initialize <b>ALL</b> network placeholders IDs", btnStyle))
            {
                SetPlaceholdersNewIDs(true);
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

        private void SetPlaceholdersNewIDs(bool overrideAll = false)
        {
            System.Random rnd = new();

            if (overrideAll)
            {
                networkPlaceholders.ForEach(p =>
                {
                    ((INetworkPlaceholder)p).InitializationId = rnd.Next(1, 99999);
                    EditorUtility.SetDirty(p);
                });
            }
            else
            {
                // Here are stored the recalculated (or preserved if correct) previous IDs during the update session.
                List<int> updatedIds = new List<int>();

                // First of all, get distinct of all used IDs greater than 0.
                List<int> oldIds = networkPlaceholders.Select(p => ((INetworkPlaceholder)p).InitializationId).Distinct().Where(id => id > 0).ToList();

                // Then, for each value, keep it if it is the first time it appears.
                // Else, if it is duplicated or 0, then recalculate it with a brand new value.
                networkPlaceholders.ForEach(p =>
                {
                    int currentId = ((INetworkPlaceholder)p).InitializationId;

                    bool needsOverride = currentId == 0 || !oldIds.Contains(currentId);

                    if (needsOverride)
                    {
                        var newId = rnd.Next(1, 99999);
                        while (oldIds.Contains(newId) || updatedIds.Contains(newId))
                        {
                            // Recalculate if this ID is already used!
                            newId = rnd.Next(1, 99999);
                        }

                        ((INetworkPlaceholder)p).InitializationId = newId;
                        EditorUtility.SetDirty(p);
                    }
                    else
                    {
                        oldIds.Remove(currentId);
                    }

                    updatedIds.Add(((INetworkPlaceholder)p).InitializationId);
                });
            }
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

                    string tmpCodeStr = tmpCode.ToString();
                    string tmpFeedback = string.Empty;

                    if (tmpCode != 0)
                    {
                        if (usedCodes.TryGetValue(tmpCode, out string alreadyUsedGOName))
                        {
                            // Code already used.
                            tmpCodeStr = "<color=red>" + tmpCodeStr + "</color>";
                            tmpFeedback = "Used by: <b>" + alreadyUsedGOName + "</b>";
                        }
                        else
                        {
                            usedCodes.Add(tmpCode, placeholder.gameObject.name);
                        }
                    }
                    else
                    {
                        // Not inizialized.
                        tmpCodeStr = "<color=red>" + tmpCodeStr + "</color>";
                        tmpFeedback = "This GameObject is <b>not initialized</b>.";
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
