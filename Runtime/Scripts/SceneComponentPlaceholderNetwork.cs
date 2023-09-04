using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class SceneComponentPlaceholderNetwork : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private bool isNetworked;
        [SerializeField] private int initializationId;

        public bool IsNetworked { get => isNetworked; set => isNetworked = value; }
        public int InitializationId { get => initializationId; set => initializationId = value; }

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
