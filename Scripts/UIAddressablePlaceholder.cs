using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class UIAddressablePlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;

        public string AddressableKey => addressableKey;
    }
}

