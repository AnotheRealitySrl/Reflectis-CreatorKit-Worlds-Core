using System;
using System.Threading.Tasks;

using UnityEngine;

using static Virtuademy.Placeholders.SceneComponentsMapper;

namespace Virtuademy.Placeholders
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] protected ERuntimeComponentId componentId;
        [SerializeField] protected bool isNetworked = true;

        public virtual async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(componentId))
            {
                if (!typeof(INetworkRuntimeComponent).IsAssignableFrom(type) || (typeof(INetworkRuntimeComponent).IsAssignableFrom(type) && isNetworked))
                {
                    await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
                }
            }
        }
    }
}