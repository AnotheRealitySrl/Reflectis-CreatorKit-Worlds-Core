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
        public enum EPOITextFontStyle
        {
            Normal,
            Bold
        }

        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text body;

        [Space]
        [Header("Configurable stuff")]

        [SerializeField, Tooltip("Choose whether to display an header or not.")]
        [OnChangedCall(nameof(OnHeaderVisibilityChanged))]
        private bool showHeader = true;

        [SerializeField, Tooltip("Configure the text that will be shown.")]
        [OnChangedCall(nameof(OnHeaderTextChanged))]
        private string headerText;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnHeaderFontSizeChanged))]
        private float headerFontSize;


        [SerializeField, Tooltip("Configure the text that will be shown.")]
        [OnChangedCall(nameof(OnBodyTextChanged))]
        private string bodyText;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnBodyFontSizeChanged))]
        private float bodyFontSize;

        public bool ShowHeader => showHeader;

        public string HeaderText => headerText;
        public float FontSize => headerFontSize = 2f;

        public string BodyText => bodyText;
        public float BodyFontSize => bodyFontSize = 1f;


        public void OnHeaderVisibilityChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(header.gameObject);
            so.FindProperty("m_IsActive").boolValue = showHeader;
            so.ApplyModifiedProperties();
#endif
        }

        public void OnHeaderTextChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(header);
            so.FindProperty("m_text").stringValue = headerText;
            so.ApplyModifiedProperties();
#endif
        }

        public void OnHeaderFontSizeChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(header);
            so.FindProperty("m_fontSize").floatValue = headerFontSize / 10f;
            so.ApplyModifiedProperties();
#endif
        }

        public void OnBodyTextChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(body);
            so.FindProperty("m_text").stringValue = bodyText;
            so.ApplyModifiedProperties();
#endif
        }

        public void OnBodyFontSizeChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(body);
            so.FindProperty("m_fontSize").floatValue = bodyFontSize / 10f;
            so.ApplyModifiedProperties();
#endif
        }

    }
}
