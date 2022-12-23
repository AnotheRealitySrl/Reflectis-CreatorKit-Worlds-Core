using SPACS.SDK.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Virtuademy/SceneComponentsMapper", fileName = "SceneComponentsMapper")]
public class SceneComponentsMapper : ScriptableObject
{
    [SerializeField] private TextAsset teleportationComponent;

    public Type TeleportationComponent => GetType(teleportationComponent.name);

    public Type GetType(string typeName)
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
