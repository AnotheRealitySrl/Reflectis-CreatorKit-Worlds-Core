using Reflectis.SDK.CreatorKit;

using Sirenix.Utilities;

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

using Virtuademy.DTO;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class NetworkPlaceholdersManagementWindow : EditorWindow
    {
        [MenuItem("Reflectis/Network placeholders management")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(NetworkPlaceholdersManagementWindow));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Initialize network placeholders IDs"))
            {
                SetAllPlaceholderNewID();
            }
        }

        private void SetAllPlaceholderNewID()
        {
            Scene s = SceneManager.GetActiveScene();
            GameObject[] gameObjects = s.GetRootGameObjects();

            List<SceneComponentPlaceholderNetwork> placeholders = new();
            List<SpawnNetworkedAddressablePlaceholder> addressablePlaceholders = new();

            gameObjects.ForEach(x => placeholders.AddRange(x.GetComponentsInChildren<SceneComponentPlaceholderNetwork>(true)));
            gameObjects.ForEach(x => addressablePlaceholders.AddRange(x.GetComponentsInChildren<SpawnNetworkedAddressablePlaceholder>(true)));

            if (placeholders.Count != 0)
            {
                for (var i = 0; i < placeholders.Count; i++)
                {
                    if (placeholders[i].IsNetworked)
                    {
                        placeholders[i].InitializationId = i + 1;

                        EditorUtility.SetDirty(placeholders[i]);
                    }


                    if (i == placeholders.Count - 1)
                    {
                        for (var j = 0; j < addressablePlaceholders.Count; j++)
                        {
                            if (addressablePlaceholders[j].IsNetworked)
                            {
                                addressablePlaceholders[j].InitializationId = j + i + 2;
                                EditorUtility.SetDirty(addressablePlaceholders[j]);
                            }
                        }
                    }
                }
            }
            else
            {
                for (var j = 0; j < addressablePlaceholders.Count; j++)
                {
                    if (addressablePlaceholders[j].IsNetworked)
                    {
                        addressablePlaceholders[j].InitializationId = j + 1;
                        EditorUtility.SetDirty(addressablePlaceholders[j]);
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            foreach (var placeholder in placeholders)
            {
                EditorGUILayout.LabelField(new GUIContent($"{placeholder.name}: {placeholder.InitializationId}"));
            }
            foreach (var placeholder in addressablePlaceholders)
            {
                EditorGUILayout.LabelField(new GUIContent($"{placeholder.name}: {placeholder.InitializationId}"));
            }
        }

    }
}
