using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] private bool automaticSetup = true;

        public bool AutomaticSetup => automaticSetup;

        public virtual async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(GetType().ToString().Split('.')[^1]))
            {
                Debug.Log($"Initializing controller \"{type}\" from placeholder \"{this}\" on gameobject {gameObject}", gameObject);
                await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
                Debug.Log($"Controller \"{type}\" correctly initialized on gameobject {gameObject}", gameObject);
            }
        }

    }
}