using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        public virtual async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(GetType().ToString().Split('.')[^1]))
            {
                await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
            }
        }
    }
}