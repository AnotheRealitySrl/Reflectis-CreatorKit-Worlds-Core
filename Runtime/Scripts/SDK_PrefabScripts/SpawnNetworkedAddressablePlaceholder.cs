#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;

using Reflectis.SDK.CreatorKit;

namespace Virtuademy.DTO
{
    public class SpawnNetworkedAddressablePlaceholder : SpawnAddressablePlaceholder
    {
        [SerializeField] private bool isNetworked;

#if ODIN_INSPECTOR
        [ShowIf("IsNetworked")]
#endif
        [SerializeField] private string addressableKeyNetwork;

#if ODIN_INSPECTOR
        [ShowIf("IsNetworked")]
#endif
        [SerializeField] private int initializationId;

        public bool IsNetworked => isNetworked;
        public string AddressableKeyNetwork => addressableKeyNetwork;
        public int InitializationId { get => initializationId; set => initializationId = value; }
    }

}
