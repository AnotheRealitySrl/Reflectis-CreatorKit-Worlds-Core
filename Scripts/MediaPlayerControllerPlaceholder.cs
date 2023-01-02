using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEditor.Rendering;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Virtuademy.DTO;
using Virtuademy.UI;

namespace Virtuademy.Placeholders
{
    public class MediaPlayerControllerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;
        [SerializeField] private int initializationId;

        [Header("Big screen")]
        [SerializeField] private bool isBigScreen;

        [Header("Media player references")]
        [SerializeField] private GameObject header;
        [SerializeField] private Transform screenContainer;
        [SerializeField] private GameObject loadingIcon;
        [SerializeField] private TextMeshProUGUI mediaLabel;
        [SerializeField] private TextMeshProUGUI mediaOwner;
        [SerializeField] private Image mediaTypeIcon;
        [SerializeField] private GameObject closeButton;

        [Header("Media player graphics")]
        [SerializeField] private GameObject buttonsPanel;
        [SerializeField] private List<Sprite> controllerSprites = new();
        [SerializeField] private List<UnityEvent> controllerCallbacks = new();
        [SerializeField] private Button controllerButtonTemplate;
        [SerializeField] private GameObject controllerButtonSeparator;

        [Header("Confirmation popup")]
        [SerializeField] private ConfirmationPopupPanel popup;
        [SerializeField] private ConfirmationPopupLocalizedText closeMediaLocalizedText;
        [SerializeField] private ConfirmationPopupLocalizedText sendMediaToScreenLocalizedText;

        public Role OwnerhsipMask => OwnerhsipMask;
        public int InitializationId => initializationId;

        public bool IsBigScreen => isBigScreen;

        public GameObject Header => header;
        public Transform ScreenContainer => screenContainer;
        public GameObject LoadingIcon => loadingIcon;
        public TextMeshProUGUI MediaLabel => mediaLabel;
        public TextMeshProUGUI MediaOwner => mediaOwner;
        public Image MediaTypeIcon => mediaTypeIcon;
        public GameObject CloseButton => closeButton;

        public GameObject ButtonsPanel => buttonsPanel;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<UnityEvent> ControllerCallbacks => controllerCallbacks;
        public Button ControllerButtonTemplate => controllerButtonTemplate;
        public GameObject ControllerButtonSeparator => controllerButtonSeparator;

        public ConfirmationPopupPanel Popup => popup;
        public ConfirmationPopupLocalizedText CloseMediaLocalizedText => closeMediaLocalizedText;
        public ConfirmationPopupLocalizedText SendMediaToScreenLocalizedText => sendMediaToScreenLocalizedText;
    }
}
