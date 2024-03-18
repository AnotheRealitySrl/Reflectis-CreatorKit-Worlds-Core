#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;

using Reflectis.SDK.CreatorKit;

using UnityEngine.Serialization;

namespace Reflectis.DTO
{
    public class SpawnNetworkedAddressablePlaceholder : SpawnAddressablePlaceholder, INetworkPlaceholder
    {
#if ODIN_INSPECTOR
        [ShowIf("IsNetworked")]
#endif
        [SerializeField] private string addressableKeyNetwork;
        [SerializeField, HideInInspector] private int initializationId;

        [field: SerializeField, FormerlySerializedAs("isNetworked")] public bool IsNetworked { get; set; }
        public int InitializationId { get => initializationId; set => initializationId = value; }
        public string AddressableKeyNetwork => addressableKeyNetwork;
    }
}
