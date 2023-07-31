using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class UIAddressablePlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;

        public string AddressableKey => addressableKey;
    }
}

