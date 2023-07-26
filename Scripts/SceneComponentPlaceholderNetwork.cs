using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SceneComponentPlaceholderNetwork : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private bool isNetworked;
        [SerializeField] private int initializationId;
        [SerializeField] private Role ownershipMask;

        public bool IsNetworked => isNetworked;
        public int InitializationId { get => initializationId; set => initializationId = value; }
        public Role OwnershipMask { get => ownershipMask; set => ownershipMask = value; }

        public override async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(GetType().ToString().Split('.')[^1]))
            {
                if (!typeof(INetworkRuntimeComponent).IsAssignableFrom(type) || (typeof(INetworkRuntimeComponent).IsAssignableFrom(type) && IsNetworked))
                {
                    await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
                }
            }
        }
    }
}
