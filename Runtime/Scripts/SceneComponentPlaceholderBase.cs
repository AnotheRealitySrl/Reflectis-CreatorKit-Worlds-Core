using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
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

        [ContextMenu("Set All Placeholder New ID")]
        private void SetAllPlaceholderNewID()
        {
            var placeholders = FindObjectsOfType<SceneComponentPlaceholderNetwork>();

            for (var i = 0; i < placeholders.Length; i++)
            {
                if (placeholders[i].IsNetworked)
                {
                    placeholders[i].InitializationId = i + 1;
                }
            }
        }

    }
}