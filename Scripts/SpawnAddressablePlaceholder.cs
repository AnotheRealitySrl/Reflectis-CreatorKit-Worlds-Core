using Sirenix.OdinInspector;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpawnAddressablePlaceholder : SceneComponentPlaceholderNetwork
    {
        [SerializeField] private string addressableKey;
        [SerializeField, ShowIf("IsNetworked")] private string addressableKeyNetwork;
        [SerializeField] private Transform parent;

        public string AddressableKey => addressableKey;
        public string AddressableKeyNetwork => addressableKeyNetwork;
        public Transform Parent => parent;
    }

}
