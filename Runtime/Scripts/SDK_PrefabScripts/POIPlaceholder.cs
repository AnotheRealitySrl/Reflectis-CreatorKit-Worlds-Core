using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIPlaceholder : MonoBehaviour
    {
        [SerializeField, Tooltip("The title of the POI.")]
        private string title;

        [SerializeField, Tooltip("The text of the POI.")]
        private string text;

        [SerializeField, Tooltip("The image of the POI.")]
        private Sprite image;

        public string Title => title;
        public string Text => text;
        public Sprite Image => image;
    }
}
