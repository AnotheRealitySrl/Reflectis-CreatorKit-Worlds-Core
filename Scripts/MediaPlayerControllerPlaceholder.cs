using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaPlayerControllerPlaceholder : UIAddressablePlaceholder
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;
        [SerializeField] private int initializationId;

        public Role OwnershipMask => ownershipMask;
        public int InitializationId => initializationId;
    }
}
