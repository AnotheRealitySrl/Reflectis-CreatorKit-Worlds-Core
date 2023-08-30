using Reflectis.SDK.Utilities.Extensions;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "AnotheReality/Virtuademy/SceneComponentsMapper", fileName = "SceneComponentsMapper")]
    public class SceneComponentsMapper : SerializedScriptableObject
    {
        // This Dictionary will be serialized by Odin.
        [SerializeField] private Dictionary<TextAsset, List<TextAsset>> componentsMap = new();

        public List<Type> GetComponentsTypes(string id)
        {
            if (componentsMap.Keys.Count(x => x.name == id) > 0)
            {
                Debug.Log("Mapping for component \"" + id + "\": Mapped");
                return componentsMap[componentsMap.Keys.FirstOrDefault(x => x.name == id)].Select(x => GetType(x.name)).ToList();
            }
            else
            {
                Debug.LogWarning("Mapping for component \"" + id + "\": Not mapped");
                return new List<Type>() { };
            }
        }

        private Type GetType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.LastPartOfTypeName() == typeName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}