using UnityEditor;

using UnityEngine;

using static Virtuademy.Environments.ScriptingApi.Interaction.IContextualMenuManageable;
using static Virtuademy.Environments.ScriptingApi.Interaction.IInteractable;


namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    [CustomEditor(typeof(InteractablePlaceholderObsolete))]
    public class InteractablePlaceholderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            InteractablePlaceholderObsolete interactablePlaceholder = target as InteractablePlaceholderObsolete;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticSetup"));
            // IsNetworked is a property -> <IsNetworked>k__BackingField
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<IsNetworked>k__BackingField"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionColliders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionModes"));

            if (interactablePlaceholder.InteractionModes.HasFlag(EInteractableType.Manipulable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("manipulationMode"));
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

            if (interactablePlaceholder.InteractionModes.HasFlag(EInteractableType.VisualScriptingInteractable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vrVisualScriptingInteraction"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lockHoverDuringInteraction"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionsScriptMachine"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("desktopAllowedStates"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vrAllowedStates"));
            }

            if (interactablePlaceholder.InteractionModes.HasFlag(EInteractableType.ContextualMenuInteractable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("contextualMenuOptions"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("contextualMenuType"));
                if (interactablePlaceholder.ContextualMenuOptions.HasFlag(EContextualMenuOption.NonProportionalScale))
                {
                    bool hasWeirdCollider = false;
                    foreach (var collider in interactablePlaceholder.GetComponentsInChildren<Collider>())
                    {
                        if (collider is not BoxCollider || collider is not MeshCollider)
                        {
                            hasWeirdCollider = true;
                            break;
                        }
                    }
                    if (hasWeirdCollider)
                    {
                        EditorGUILayout.HelpBox("Non proportional scale option can cause unexpected behaviours if associated with any " +
                            "collider that cannot be scaled in non proportional ways (es. Sphere Collider, Capsule Collider ...)", MessageType.Warning);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}
