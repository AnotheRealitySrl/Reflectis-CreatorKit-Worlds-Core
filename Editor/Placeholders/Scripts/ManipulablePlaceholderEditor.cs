using UnityEditor;

namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    [CustomEditor(typeof(ManipulablePlaceholder))]
    public class ManipulablePlaceholderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ManipulablePlaceholder interactablePlaceholder = target as ManipulablePlaceholder;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("manipulationMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("vrInteraction"));
            if (interactablePlaceholder.VrInteraction != 0)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dynamicAttach"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("adjustRotationOnRelease"));

                if (interactablePlaceholder.AdjustRotationOnRelease)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("realignAxisX"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("realignAxisY"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("realignAxisZ"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("realignDurationTimeInSeconds"));
                }
            }

            if (interactablePlaceholder.ManipulationMode.HasFlag(Virtuademy.Environments.ScriptingApi.Interaction.IManipulable.EManipulationMode.Rotate))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmosEnabled"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("threshold"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("freeRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lockXRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lockYRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isFocusedInteractable"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
