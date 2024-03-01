using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Serialization;

namespace Reflectis.SDK.CreatorKit
{
    public class SceneComponentPlaceholderNetwork : SceneComponentPlaceholderBase, INetworkPlaceholder
    {
        [SerializeField, HideInInspector] private int initializationId;

        [field: SerializeField, FormerlySerializedAs("isNetworked")] public bool IsNetworked { get; set; }
        public int InitializationId { get => initializationId; set => initializationId = value; }

        public override async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(GetType().ToString().Split('.')[^1]))
            {
                if (!typeof(INetworkRuntimeComponent).IsAssignableFrom(type) || (typeof(INetworkRuntimeComponent).IsAssignableFrom(type) && IsNetworked && InitializationId != 0))
                {
                    Debug.Log($"Initializing network controller \"{type}\" from placeholder \"{this}\" on gameobject {gameObject}", gameObject);
                    await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
                    Debug.Log($"Network controller \"{type}\" correctly initialized on gameobject {gameObject}", gameObject);
                }
            }
        }
    }
}
