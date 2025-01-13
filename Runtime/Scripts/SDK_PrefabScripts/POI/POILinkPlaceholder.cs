using Reflectis.SDK.Utilities;

using System;

using TMPro;


using UnityEditor;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [Serializable]
    public class POILinkPlaceholder : POIBlockPlaceholder
    {
        [Space]
        [Header("Configurable stuff")]

        [SerializeField, Tooltip("Add a link that will be opened by the application")]
        private string link;

        [SerializeField, Tooltip("A short text that provides info to the link, like \"see more\" or \"download\".")]
        [OnChangedCall(nameof(OnTextChanged))]
        private string text;

        [SerializeField, Tooltip("Change the font size of the text.")]
        [OnChangedCall(nameof(OnFontSizeChanged))]
        private float fontSize = 1f;

        public string Link => link;
        public string Text => text;
        public float FontSize => fontSize;


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
            so.FindProperty("m_fontSize").floatValue = fontSize / 10f;
            so.ApplyModifiedProperties();
#endif
        }
    }
}
