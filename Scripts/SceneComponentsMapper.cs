using Reflectis.SDK.Utilities.Extensions;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    [CreateAssetMenu(menuName = "AnotheReality/Virtuademy/SceneComponentsMapper", fileName = "SceneComponentsMapper")]
    public class SceneComponentsMapper : SerializedScriptableObject
    {
        // This Dictionary will be serialized by Odin.
        [SerializeField] private Dictionary<TextAsset, List<TextAsset>> componentsMap = new();

        public List<Type> GetComponentsTypes(string id) => componentsMap[componentsMap.Keys.First(x => x.name == id)].Select(x => GetType(x.name)).ToList();

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