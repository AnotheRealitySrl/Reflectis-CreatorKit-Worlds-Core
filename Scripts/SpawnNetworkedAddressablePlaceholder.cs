using Sirenix.OdinInspector;

using UnityEngine;

using Virtuademy.Placeholders;

namespace Virtuademy.DTO
{
    public class SpawnNetworkedAddressablePlaceholder : SpawnAddressablePlaceholder
    {
        [SerializeField] private bool isNetworked;
        [SerializeField, ShowIf("IsNetworked")] private string addressableKeyNetwork;

        public bool IsNetworked => isNetworked;
        public string AddressableKeyNetwork => addressableKeyNetwork;
    }

}
