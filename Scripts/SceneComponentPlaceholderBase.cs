using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Virtuademy.Placeholders.SceneComponentsMapper;

namespace Virtuademy.Placeholders
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] protected ERuntimeComponentId componentId;

        public virtual void Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(componentId))
            {
                ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
            }
        }
    }
}