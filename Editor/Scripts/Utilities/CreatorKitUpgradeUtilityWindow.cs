using Reflectis.SDK.CreatorKit;
using Reflectis.SDK.Utilities.Extensions;

using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public class CreatorKitUpgradeWindow : EditorWindow
    {
        private PlaceholderUpgradeUtility placeholderUpgradeUtility;
        private SceneComponentPlaceholderBase[] legacyPlaceholders;

        [MenuItem("Reflectis/Upgrade obsolete components")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(CreatorKitUpgradeWindow));
        }

        private void OnGUI()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true,
            };

            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox($"Drag and drop here the placeholder update utilities you want to use.", MessageType.Info);
            placeholderUpgradeUtility = EditorGUILayout.ObjectField(placeholderUpgradeUtility, typeof(PlaceholderUpgradeUtility), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as PlaceholderUpgradeUtility;
            EditorGUILayout.EndVertical();

            if (placeholderUpgradeUtility)
            {
                //if (legacyPlaceholders == null || legacyPlaceholders.Length == 0)
                //{
                Type type = GetType(placeholderUpgradeUtility.LegacyPlaceholder.name);
                legacyPlaceholders = FindObjectsOfType(type, true) as SceneComponentPlaceholderBase[];
                //}

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{(legacyPlaceholders.Length == 0 ? "<b>[<color=lime>√</color>]</b>" : "<b>[<color=red>X</color>]</b>")}", style, GUILayout.Width(20));
                EditorGUILayout.LabelField($"Found {legacyPlaceholders.Count()} legacy placeholders.", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Upgrade placeholders", GUILayout.ExpandWidth(false)))
                {
                    placeholderUpgradeUtility.Upgrade();
                }
                EditorGUILayout.EndHorizontal();

                foreach (SceneComponentPlaceholderBase placeholder in legacyPlaceholders)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"> {placeholder.name}");
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

        }

        private Type GetType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.LastPartOfTypeName() == typeName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

    }
}
