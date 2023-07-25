using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using static Virtuademy.Placeholders.SceneComponentsMapper;

namespace Virtuademy.Placeholders
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] protected ERuntimeComponentId componentId;
        [SerializeField] protected bool isNetworked = true;
        [SerializeField] protected int initializationId;

        public bool IsNetworked => isNetworked;
        public int InitializationId { get => initializationId; set => initializationId = value; }

        public virtual void Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(componentId))
            {
                if (!typeof(INetworkRuntimeComponent).IsAssignableFrom(type) || (typeof(INetworkRuntimeComponent).IsAssignableFrom(type) && isNetworked))
                {
                    ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
                }
            }
        }

        [ContextMenu("Set All Placeholder New ID")]
        private void SetAllPlaceholderNewID()
        {
            var placeholders = FindObjectsOfType<SceneComponentPlaceholderBase>();

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