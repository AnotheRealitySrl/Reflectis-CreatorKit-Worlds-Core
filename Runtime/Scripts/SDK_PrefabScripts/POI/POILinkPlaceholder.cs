using Reflectis.SDK.Utilities;

using System;

using TMPro;


using UnityEditor;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [Serializable]
    public class POILinkPlaceholder : POIBlockPlaceholder
    {
        [SerializeField, Tooltip("Add a link that will be opened by the application")]
        private string link;

        [SerializeField, Tooltip("Add text that will be shown in the POI.")]
        [OnChangedCall(nameof(OnTextChanged))]
        private string text;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnFontSizeChanged))]
        private float fontSize;

        public override string AddressableKey => "POILinkBlock";

        public string Link => link;
        public string Text => text;
        public float FontSize => fontSize = 1f;


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
