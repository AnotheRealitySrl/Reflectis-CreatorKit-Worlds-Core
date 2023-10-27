using UnityEngine;
using UnityEngine.Video;
using Virtuademy.DTO;

namespace Reflectis.SDK.CreatorKit
{
    public class MediaContainerPlaceholder : SpawnNetworkedAddressablePlaceholder
    {
        [SerializeField] private string mediaUrl;
        [SerializeField] private VideoClip clip;
        [SerializeField] private bool isActive;
        [SerializeField] private bool disableAtStart = false;
        [SerializeField] private GameObject externalController;
        [SerializeField] private GameObject buttonTemplate;
        [SerializeField] private GameObject separatorTemplate;
        [SerializeField] private GameObject selectionVideoImage;

        public string MediaUrl => mediaUrl;
        public bool IsActive => isActive;
        public bool DisableAtStart => disableAtStart;
        public GameObject ExternalController => externalController;
        public GameObject ButtonTemplate => buttonTemplate;
        public GameObject SeparatorTemplate => separatorTemplate;
        public VideoClip Clip => clip;
        public GameObject SelectionVideoImage => selectionVideoImage;
    }
}
