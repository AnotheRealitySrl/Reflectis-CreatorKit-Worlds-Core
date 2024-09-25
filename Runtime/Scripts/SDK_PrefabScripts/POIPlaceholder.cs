using Reflectis.SDK.Utilities;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{
    public class POIPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Do not change these references.")]
        [SerializeField] private Transform titleContainer;
        [SerializeField] private Transform textContainer;
        [SerializeField] private Transform imageContainer;

        [Header("Title configuration")]
        [SerializeField, Tooltip("The title of the POI.")]
        [OnChangedCall(nameof(OnTitleChanged))]
        private string title;

        [SerializeField, Tooltip("Is the background of the title visible?")]
        [OnChangedCall(nameof(OnTitleBackgroundChanged))]
        private bool titleBackgroundVisible = true;


        [Header("Text configuration")]
        [SerializeField, Tooltip("The text of the POI.")]
        [OnChangedCall(nameof(OnTextChanged))]
        private string text;

        [SerializeField, Tooltip("Is the background of the text visible?")]
        [OnChangedCall(nameof(OnTextBackgroundChanged))]
        private bool textBackgroundVisible = true;


        [Header("Image configuration")]
        [SerializeField, Tooltip("The image of the POI.")]
        [OnChangedCall(nameof(OnImageChanged))]
        private Sprite image;


        public Transform TitleContainer => titleContainer;
        public Transform TextContainer => textContainer;
        public Transform ImageContainer => imageContainer;

        public string Title => title;
        public bool TitleBackgroundVisible => titleBackgroundVisible;

        public string Text => text;
        public bool TextBackgroundVisible => textBackgroundVisible;

        public Sprite Image => image;


        public void OnTitleChanged()
        {
            TitleContainer.GetComponentInChildren<TMP_Text>().text = title;
        }

        public void OnTitleBackgroundChanged()
        {
            TitleContainer.GetComponent<Image>().enabled = titleBackgroundVisible;
        }

        public void OnTextChanged()
        {
            TextContainer.GetComponentInChildren<TMP_Text>().text = text;
        }

        public void OnTextBackgroundChanged()
        {
            TextContainer.GetComponent<Image>().enabled = titleBackgroundVisible;
        }

        public void OnImageChanged()
        {
            ImageContainer.GetChild(0).GetComponent<Image>().sprite = image;
        }
    }
}
