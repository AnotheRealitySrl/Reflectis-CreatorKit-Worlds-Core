using TMPro;

using UnityEditor;

using UnityEngine;

namespace Virtuademy.Environments.ScriptingApi.Placeholders.Editor
{
    /// <summary>
    /// Keeps the POI placeholder text fields aligned with the TMP components in the placeholder
    /// hierarchy. The inspector fields already push their value into the TMP on edit (OnChangedCall
    /// callbacks on the placeholders); these editors close the loop in the other direction: when a
    /// creator typed into the TMP directly, selecting the placeholder pulls the TMP text back into
    /// the serialized field, so the inspector always shows what is actually displayed. At runtime
    /// the placeholder text getters read the TMP as the source of truth, so the two never diverge.
    /// </summary>
    internal static class POIPlaceholderTextSync
    {
        public static void PullFromTMP(Object target, string fieldName, TMP_Text tmp)
        {
            if (tmp == null)
                return;

            SerializedObject so = new(target);
            SerializedProperty property = so.FindProperty(fieldName);
            if (property != null && property.stringValue != tmp.text)
            {
                property.stringValue = tmp.text;
                so.ApplyModifiedProperties();
            }
        }
    }

    // Inherits NetworkPlaceholderEditor so the networked-placeholder debug section it draws for
    // every SceneComponentPlaceholderBase subclass is not lost to this more specific editor.
    [CustomEditor(typeof(POIPlaceholder))]
    [CanEditMultipleObjects]
    public class POIPlaceholderEditor : NetworkPlaceholderEditor
    {
        private void OnEnable()
        {
            foreach (Object target in targets)
            {
                POIPlaceholder placeholder = (POIPlaceholder)target;
                TMP_Text titleTMP = placeholder.Title != null ? placeholder.Title.GetComponentInChildren<TMP_Text>(true) : null;
                POIPlaceholderTextSync.PullFromTMP(target, "titleText", titleTMP);
            }
        }
    }

    [CustomEditor(typeof(POITextPlaceholder))]
    [CanEditMultipleObjects]
    public class POITextPlaceholderEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            foreach (Object target in targets)
            {
                SerializedObject so = new(target);
                POIPlaceholderTextSync.PullFromTMP(target, "headerText", so.FindProperty("header")?.objectReferenceValue as TMP_Text);
                POIPlaceholderTextSync.PullFromTMP(target, "bodyText", so.FindProperty("body")?.objectReferenceValue as TMP_Text);
            }
        }
    }

    [CustomEditor(typeof(POILinkPlaceholder))]
    [CanEditMultipleObjects]
    public class POILinkPlaceholderEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            foreach (Object target in targets)
            {
                POILinkPlaceholder placeholder = (POILinkPlaceholder)target;
                POIPlaceholderTextSync.PullFromTMP(target, "text", placeholder.GetComponentInChildren<TMP_Text>(true));
            }
        }
    }
}
