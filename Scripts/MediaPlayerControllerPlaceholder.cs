using System;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaPlayerControllerPlaceholder : UIAddressablePlaceholder
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
        [SerializeField] private bool headerVisibility = true;
        [SerializeField] private string defaultMedia;
        [SerializeField] private MediaType type;

        public MediaTypesAddressablesDictionary MediaTypesDictionary => mediaTypesDictionary;
        public bool IsBigScreen => isBigScreen;
        public bool HeaderVisibility => headerVisibility;
        public string DefaultMedia => defaultMedia;
        public MediaType Type => type;


        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        public Role OwnershipMask => ownershipMask;
    }
}
