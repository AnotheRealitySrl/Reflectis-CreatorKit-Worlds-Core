using Reflectis.SDK.Utilities;

using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIPlaceholder : SpawnAddressablePlaceholder
    {
        public enum ETitleVisibility
        {
            AlwaysHidden,
            VisibleOnHover,
            AlwaysVisible
        }

        [Space]

        [Header("Read only references. Do no edit them, unless you need to do some heavy customizations.")]
        [SerializeField] private RectTransform activator;
        [SerializeField] private RectTransform title;
        [SerializeField] private RectTransform panel;
        [SerializeField] private Transform bindingLineStart;
        [SerializeField] private Transform bindingLineEnd;

        [Space]

        [Header("POI title configuration")]

        [SerializeField, Tooltip("Set the visibility of the title. " +
            "If AlwaysVisible, the title will always be visible. " +
            "If VisibleOnHover, the title will be visible only when hovering on the POI activator. " +
            "If AlwaysHidden, the title is deactivated")]
        private ETitleVisibility titleVisibility = ETitleVisibility.VisibleOnHover;

        [SerializeField, Tooltip("Change the font size.")]
        [OnChangedCall(nameof(OnPOITitleTextChanged))]
        private string titleText;

        [SerializeField, Tooltip("Change the font of the title.")]
        [OnChangedCall(nameof(OnPOITitleFontChanged))]
        private float titleFontSize = 2;

        [Space]

        [Header("POI binding line")]

        [SerializeField, Tooltip("Choose whether to draw the binding line or not.")]
        private bool bindingLineVisibility;

        [SerializeField, Tooltip("The color of the binding line.")]
        private Color bindingLineColor = Color.red;

        [SerializeField, Tooltip("The width of the binding line.")]
        private float bindingLineWidth = 1f;

        [Space]

        [HelpBox("To edit the content of each block of the POI, please double-click on the item in the list." +
            "You will be redirected to the placeholder where you can customize the content.", HelpBoxMessageType.Info)]

        [SerializeField] private List<POIBlockPlaceholder> poiElements;

        public List<POIBlockPlaceholder> POIElements => poiElements;

        public Transform Activator => activator;
        public Transform Title => title;
        public Transform Panel => panel;
        public Transform BindingLineStart => bindingLineStart;
        public Transform BindingLineEnd => bindingLineEnd;

        public ETitleVisibility TitleVisibility => titleVisibility;
        public string TitleText => titleText;
        public float TitleFontSize => titleFontSize;

        public bool BindingLineVisibility => bindingLineVisibility;
        public Color BindingLineColor => bindingLineColor;
        public float BindingLineWidth => bindingLineWidth;


        public void OnPOITitleTextChanged()
        {
            title.GetComponent<TMP_Text>().text = titleText;
        }

        public void OnPOITitleFontChanged()
        {
            title.GetComponent<TMP_Text>().fontSize = titleFontSize;
        }

        // This shows a preview of the binding line
        void OnDrawGizmosSelected()
        {
            if (bindingLineVisibility)
            {
                Gizmos.color = bindingLineColor;
                Gizmos.DrawLine(bindingLineStart.position, bindingLineEnd.position);
            }
        }
    }
}
