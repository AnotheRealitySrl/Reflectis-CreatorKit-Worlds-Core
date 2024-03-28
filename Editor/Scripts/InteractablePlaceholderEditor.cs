using Reflectis.SDK.CreatorKit;

using UnityEditor;

namespace Reflectis.SDK.CreatorKitEditor
{
    [CustomEditor(typeof(InteractablePlaceholder))]
    public class InteractablePlaceholderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InteractablePlaceholder interactablePlaceholder = target as InteractablePlaceholder;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionColliders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionModes"));

            if (interactablePlaceholder.InteractionModes.HasFlag(InteractionNew.IInteractable.EInteractableType.Manipulable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("manipulationMode"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("nonProportionalScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vrInteraction"));
                if (interactablePlaceholder.VRInteraction != 0)
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
            }

            if (interactablePlaceholder.InteractionModes.HasFlag(InteractionNew.IInteractable.EInteractableType.GenericInteractable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionsScriptMachine"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("needUnselectOnDestroyScriptMachine"));

                if (interactablePlaceholder.NeedUnselectOnDestroyScriptMachine)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("unselectOnDestroyScriptMachine"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("desktopAllowedStates"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vrAllowedStates"));
            }

            if (interactablePlaceholder.InteractionModes.HasFlag(InteractionNew.IInteractable.EInteractableType.ContextualMenuInteractable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("contextualMenuOptions"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("contextualMenuType"));
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}
