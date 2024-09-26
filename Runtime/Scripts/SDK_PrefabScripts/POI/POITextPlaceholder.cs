using Reflectis.SDK.Utilities;

using System;

using TMPro;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [Serializable]
    public class POITextPlaceholder : POIBlockPlaceholder
    {
        [SerializeField, Tooltip("Add text that will be shown in the POI.")]
        [OnChangedCall(nameof(OnTextChanged))]
        private string text;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnFontSizeChanged))]
        private float fontSize;

        public override string AddressableKey => "POITextBlock";

        public string Text => text;
        public float FontSize => fontSize = 1f;


        public void OnTextChanged()
        {
            transform.GetComponentInChildren<TMP_Text>().text = text;
        }

        public void OnFontSizeChanged()
        {
            transform.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
        }
    }
}
