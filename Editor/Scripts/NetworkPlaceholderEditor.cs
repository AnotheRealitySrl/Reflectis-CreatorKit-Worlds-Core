using Reflectis.CreatorKit.Core;

using UnityEditor;

using UnityEngine;

namespace Reflectis.CreatorKit.CoreEditor
{
    [CustomEditor(typeof(SceneComponentPlaceholderBase), true)]
    public class NetworkPlaceholderEditor : Editor
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
                EditorGUILayout.LabelField($"<b>Initialization id:</b> {networkPlaceholder.InitializationId}", style);

                if (networkPlaceholder.InitializationId == 0)
                {
                    EditorGUILayout.HelpBox("This network placeholder has a null ID. Go to Reflectis > Network placeholders management to assign the IDs of all components", MessageType.Error);
                }
            }
        }
    }
}
