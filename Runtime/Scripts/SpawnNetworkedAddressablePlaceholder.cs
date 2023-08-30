using Sirenix.OdinInspector;

using UnityEngine;

using Reflectis.SDK.CreatorKit;

namespace Virtuademy.DTO
{
    public class SpawnNetworkedAddressablePlaceholder : SpawnAddressablePlaceholder
    {
        [SerializeField] private bool isNetworked;
        [SerializeField, ShowIf("IsNetworked")] private string addressableKeyNetwork;
        [SerializeField, ShowIf("IsNetworked")] private int initializationId;

        public bool IsNetworked => isNetworked;
        public string AddressableKeyNetwork => addressableKeyNetwork;
        public int InitializationId => initializationId;
    }

}
