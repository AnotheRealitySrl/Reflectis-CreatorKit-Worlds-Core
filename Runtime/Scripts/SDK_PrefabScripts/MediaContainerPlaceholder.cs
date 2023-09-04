using UnityEngine;

using Virtuademy.DTO;

namespace Reflectis.SDK.CreatorKit
{
    public class MediaContainerPlaceholder : SpawnNetworkedAddressablePlaceholder
    {
        [SerializeField] private string mediaUrl;
        [SerializeField] private bool isActive;
        [SerializeField] private GameObject externalController;
        [SerializeField] private GameObject buttonTemplate;
        [SerializeField] private GameObject separatorTemplate;

        public string MediaUrl => mediaUrl;
        public bool IsActive => isActive;
        public GameObject ExternalController => externalController;
        public GameObject ButtonTemplate => buttonTemplate;
        public GameObject SeparatorTemplate => separatorTemplate;
    }
}
