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

        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        public Role OwnershipMask => ownershipMask;
        public MediaTypesAddressablesDictionary MediaTypesDictionary => mediaTypesDictionary;
    }
}
