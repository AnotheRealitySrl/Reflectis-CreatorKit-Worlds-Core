using Sirenix.OdinInspector;

using System;

using UnityEngine;

using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class MediaPlayerControllerPlaceholder : SpawnNetworkedAddressablePlaceholder
    {
        #region Enums

        /// <summary>
        /// Available media types managed by the <see cref="MediaPlayerController"/>.
        /// </summary>
        [Flags]
        public enum MediaType
        {
            Video = 1,
            Presentation = 2,
        }

        #endregion

        [Header("Media components")]
        [SerializeField] private MediaTypesAddressablesDictionary mediaTypesDictionary;
        [SerializeField] private bool isBigScreen = true;

        [Header("Header settings")]
        [SerializeField] private bool headerVisibility = true;
        [SerializeField, ShowIf("headerVisibility")] private GameObject headerContainer;

        [Header("Controller settings")]
        [SerializeField] private bool buttonsVisibility = true;
        [SerializeField, ShowIf("buttonsVisibility")] private GameObject buttonsContainer;
        [SerializeField, ShowIf("buttonsVisibility")] private string buttonsAddressable;

        [Header("Preloaded media")]
        [SerializeField] private bool defaultMedia;
        [SerializeField, ShowIf("defaultMedia")] private string mediaUrl;
        [SerializeField, ShowIf("defaultMedia")] private MediaType type;

        public MediaTypesAddressablesDictionary MediaTypesDictionary => mediaTypesDictionary;
        public bool IsBigScreen => isBigScreen;
        public bool HeaderVisibility => headerVisibility;
        public bool DefaultMedia => defaultMedia;
        public string MediaUrl => mediaUrl;
        public MediaType Type => type;
    }
}
