using Reflectis.SDK.UIKit.ToastSystem;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class UIAddressablePlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;

        [Header("Toast system")]
        [SerializeField] private InteractableByToast interactableByToast;

        public string AddressableKey => addressableKey;
        public InteractableByToast InteractableByToast => interactableByToast;
    }
}

