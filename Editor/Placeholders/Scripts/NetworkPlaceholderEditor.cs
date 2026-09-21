using Virtuademy.Environments.ScriptingApi.Placeholders;

using UnityEditor;

using UnityEngine;


namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    [CustomEditor(typeof(SceneComponentPlaceholderBase), true)]
    public class NetworkPlaceholderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneComponentPlaceholderBase placeholder = (SceneComponentPlaceholderBase)target;
            if (placeholder is INetworkPlaceholder networkPlaceholder && networkPlaceholder.IsNetworked)
            {
                GUIStyle style = new(EditorStyles.label)
                {
                    richText = true
                };

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"<b>Initialization id:</b> {placeholder.UniqueID}", style);

                if (placeholder.UniqueID == 0)
                {
                    EditorGUILayout.HelpBox("This network placeholder has a null ID. Go to Virtuademy > Network placeholders management to assign the IDs of all components", MessageType.Error);
                }
            }
        }
    }
}
