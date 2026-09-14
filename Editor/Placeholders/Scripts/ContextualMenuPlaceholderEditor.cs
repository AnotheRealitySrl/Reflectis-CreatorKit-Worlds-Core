using UnityEditor;

using UnityEngine;

using static Virtuademy.Environments.ScriptingApi.Interaction.IContextualMenuManageable;

namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    [CustomEditor(typeof(ContextualMenuPlaceholder))]
    public class ContextualMenuPlaceholderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ContextualMenuPlaceholder contextualMenuPlaceholder = target as ContextualMenuPlaceholder;
            var interactablePlaceholder = contextualMenuPlaceholder.GetComponentInChildren<InteractablePlaceholder>(true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("contextualMenuOptions"));
            if (contextualMenuPlaceholder.ContextualMenuOptions.HasFlag(EContextualMenuOption.NonProportionalScale))
            {
                bool hasWeirdCollider = false;
                if (interactablePlaceholder.InteractionColliders != null && interactablePlaceholder.InteractionColliders.Count > 0)
                {
                    foreach (var collider in interactablePlaceholder.InteractionColliders)
                    {
                        if (collider is not BoxCollider || collider is not MeshCollider)
                        {
                            hasWeirdCollider = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var collider in interactablePlaceholder.GetComponentsInChildren<Collider>())
                    {
                        if (collider is not BoxCollider || collider is not MeshCollider)
                        {
                            hasWeirdCollider = true;
                            break;
                        }
                    }
                }
                if (hasWeirdCollider)
                {
                    EditorGUILayout.HelpBox("Non proportional scale option can cause unexpected behaviours if associated with any " +
                        "collider that cannot be scaled in non proportional ways (es. Sphere Collider, Capsule Collider ...)", MessageType.Warning);
                }
            }

            if (contextualMenuPlaceholder.ContextualMenuOptions.HasFlag(EContextualMenuOption.ColorPicker))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colorFirstMeshOnly"));
            }

            if (contextualMenuPlaceholder.ContextualMenuOptions.HasFlag(EContextualMenuOption.Explodable))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("explosionMoveSubmeshes"));
            }

            if (contextualMenuPlaceholder.ContextualMenuOptions.HasFlag(EContextualMenuOption.LockTransform))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(contextualMenuPlaceholder.useSubmeshesOnLock)));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
