using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIVideoPlaceholder : POIBlockPlaceholder
    {
        [Space]
        [Header("Configurable stuff")]

        [SerializeField, Tooltip("A video must be provided with a public, non authenticated url.")]
        private string url;

        public string Url => url;
    }
}
