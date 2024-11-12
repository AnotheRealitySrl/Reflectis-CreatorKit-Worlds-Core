using UnityEngine;
using UnityEngine.Serialization;

namespace Reflectis.SDK.CreatorKit
{
    public class SceneComponentPlaceholderNetwork : SceneComponentPlaceholderBase, INetworkPlaceholder
    {
        [SerializeField, HideInInspector] private int initializationId;

        [field: SerializeField, FormerlySerializedAs("isNetworked")] public bool IsNetworked { get; set; }
        public int InitializationId { get => initializationId; set => initializationId = value; }

    }
}
