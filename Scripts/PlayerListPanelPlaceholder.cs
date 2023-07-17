using Reflectis.SDK.UIKit.ToastSystem;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class PlayerListPanelPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;

        [Header("Toast system")]
        [SerializeField] private InteractableByToast interactableByToast;

        public string AddressableKey => addressableKey;
        public InteractableByToast InteractableByToast => interactableByToast;

        //[SerializeField] private TextMeshProUGUI numberOfParticipants;
        //[SerializeField] private PlayerListPanelTile playerEntryTemplate;
        //[SerializeField] private GameObject muteButton;

        //public TextMeshProUGUI NumberOfParticipants => numberOfParticipants;
        //public PlayerListPanelTile PlayerEntryTemplate => playerEntryTemplate;
        //public GameObject MuteButton => muteButton;
    }
}

