using Reflectis.SDK.Utilities;

using System;

using TMPro;

using UnityEditor;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [Serializable]
    public class POITextPlaceholder : POIBlockPlaceholder
    {
        public enum EPOITextFontWidth
        {
            Normal,
            Bold
        }

        [SerializeField, Tooltip("Add text that will be shown in the POI.")]
        [OnChangedCall(nameof(OnTextChanged))]
        private string text;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnFontSizeChanged))]
        private float fontSize;

        [SerializeField, Tooltip("Change the font width.")]
        [OnChangedCall(nameof(OnFontSizeChanged))]
        private EPOITextFontWidth fontWidth;

        public override string AddressableKey => "POITextBlock";

        public string Text => text;
        public float FontSize => fontSize = 1f;
        public EPOITextFontWidth FontWidth => fontWidth;


        public void OnTextChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(transform.GetComponentInChildren<TMP_Text>());
            so.FindProperty("m_text").stringValue = text;
            so.ApplyModifiedProperties();
#endif
        }

        public void OnFontSizeChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(transform.GetComponentInChildren<TMP_Text>());
            so.FindProperty("m_fontSize").floatValue = fontSize;
            so.ApplyModifiedProperties();
#endif
        }
    }
}
