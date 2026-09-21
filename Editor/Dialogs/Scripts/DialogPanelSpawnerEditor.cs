using UnityEditor;

using UnityEngine;

namespace Virtuademy.SDK.Environments.Dialogs.Editor
{
    [CustomEditor(typeof(DialogPanelSpawner))]
    public class DialogPanelSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            var spawner = (DialogPanelSpawner)target;

            EditorGUILayout.LabelField("Player panel settings", EditorStyles.boldLabel);
            SerializedProperty showPlayerNickname = serializedObject.FindProperty("showPlayerNickname");
            EditorGUILayout.PropertyField(showPlayerNickname, new GUIContent("Show player nickname"));
            if (showPlayerNickname.boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useReflectisNickname"), new GUIContent("Use Virtuademy nickname"));

            SerializedProperty showPlayerAvatarContainer = serializedObject.FindProperty("showPlayerAvatarContainer");
            EditorGUILayout.PropertyField(showPlayerAvatarContainer, new GUIContent("Show player avatar"));
            if (showPlayerAvatarContainer.boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useReflectisAvatar"), new GUIContent("Use Virtuademy avatar"));

            EditorGUILayout.LabelField("Npc panel settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showNpcNickname"), new GUIContent("Show npc nickname"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showNpcAvatarContainer"), new GUIContent("Show npc avatar"));

            EditorGUILayout.LabelField("Typewrite effect settings", EditorStyles.boldLabel);
            spawner.charactersPerSecond = Mathf.Max(0, EditorGUILayout.FloatField(new GUIContent("Characters per second", "(0 = instant text)"), spawner.charactersPerSecond));
            if (spawner.charactersPerSecond > 0)
            {
                spawner.interpunctuationDelay = Mathf.Max(0, EditorGUILayout.FloatField(new GUIContent("Interpunctuation delay", "seconds"), spawner.interpunctuationDelay));

                EditorGUILayout.LabelField("Skip Settings", EditorStyles.boldLabel);
                SerializedProperty enableSkip = serializedObject.FindProperty("enableSkip");
                EditorGUILayout.PropertyField(enableSkip, new GUIContent("Enable Skip"));
                if (enableSkip.boolValue)
                {
                    SerializedProperty quickSkip = serializedObject.FindProperty("quickSkip");
                    EditorGUILayout.PropertyField(quickSkip, new GUIContent("Quick skip", "makes the text appear instantly"));
                    if (!quickSkip.boolValue)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skipSpeedup"), new GUIContent("Skip speedup", "Makes the text appear faster, multiplying this by Characters per second"));
                }
            }



            //Save changes
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(spawner);
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(spawner);
            }
        }
    }
}
