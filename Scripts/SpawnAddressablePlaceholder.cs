using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpawnAddressablePlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;
        [SerializeField] private Transform parent;

        public string AddressableKey => addressableKey;
        public Transform Parent => parent;
    }
}

